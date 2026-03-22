using UnityEngine;
using UnityEngine.AI;

namespace TinyHunter.MVP.Enemies
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class SkeletonProximityAnimator : MonoBehaviour, TinyHunter.Core.Combat.ITemporaryInvulnerabilityProvider
    {
        private static readonly int IsPlayerNearHash = Animator.StringToHash("IsPlayerNear");

        [Header("Required References")]
        [SerializeField] private Animator skeletonAnimator;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private EnemyAnimationBridge animationBridge;
        [SerializeField] private EnemyMeleeDamageHitbox meleeHitbox;
        [SerializeField] private NavMeshAgent navigationAgent;
        [SerializeField] private CapsuleCollider bodyCollider;
        [SerializeField] private Rigidbody physicsBody;

        [Header("Current MVP Detection")]
        [SerializeField] private float proximityRange = 4f;
        [SerializeField] private bool rotateTowardPlayerInRange = true;
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private bool modelForwardIsReversed = true;
        [SerializeField] private float additionalFacingYawOffset;
        [SerializeField] private bool moveTowardPlayerInRange = true;
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float stopDistance = 1.2f;
        [SerializeField] private bool useNavMeshIfAvailable = true;
        [SerializeField] private bool allowDirectMovementFallback = true;

        [Header("Solid Body / Path Blocking")]
        [SerializeField] private bool configureSolidBodyOnAwake = true;
        [SerializeField] private Vector3 colliderCenter = new(0f, 0.9f, 0f);
        [SerializeField] private float colliderHeight = 1.8f;
        [SerializeField] private float colliderRadius = 0.35f;
        [SerializeField] private LayerMask movementBlockers = Physics.DefaultRaycastLayers;
        [SerializeField] private float directMoveCastPadding = 0.05f;
        [SerializeField] private float directMoveStepHeight = 0.2f;
        [SerializeField] private float directMoveAvoidanceAngle = 35f;
        [SerializeField] private int directMoveAvoidanceChecksPerSide = 3;
        [SerializeField] private NavMeshPathStatus requiredPathStatus = NavMeshPathStatus.PathComplete;
        [SerializeField, Range(0, 99)] private int avoidancePriority = 50;
        [SerializeField] private ObstacleAvoidanceType obstacleAvoidance = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        [Header("Respawn / Spawn Intro")]
        [SerializeField] private bool playRespawnSequenceOnEnable = true;
        [SerializeField] private float respawnSequenceDuration = 1.1f;
        [SerializeField] private bool invulnerableDuringRespawn = true;
        [SerializeField] private bool useFallbackRespawnTimer = true;
        [SerializeField] private bool applyRespawnMovementOffset;
        [SerializeField] private Vector3 respawnMovementOffset = new(0f, 0f, -0.35f);
        [SerializeField] private AnimationCurve respawnMovementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Combat")]
        [SerializeField] private bool canAttack = true;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float attackCooldown = 1.25f;
        [SerializeField] private bool useFallbackAttackTimer;
        [SerializeField] private float fallbackAttackDuration = 0.9f;

        [Header("Optional Patrol")]
        [SerializeField] private bool usePatrol;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 1.5f;

        [Header("Future Detection - Disabled By Default")]
        [SerializeField] private bool useVision;
        [SerializeField] private float visionRange = 8f;
        [SerializeField, Range(1f, 180f)] private float visionAngle = 75f;
        [SerializeField] private LayerMask visionBlockers = Physics.DefaultRaycastLayers;

        [SerializeField] private bool useHearing;
        [SerializeField] private float hearingRange = 6f;
        [SerializeField] private float hearingMemoryDuration = 1.5f;

        private float lastHeardTime = float.NegativeInfinity;
        private float attackCooldownTimer;
        private float attackTimer;
        private float patrolWaitTimer;
        private int patrolIndex;
        private Vector3 lastPosition;
        private bool isRespawning;
        private float respawnTimer;
        private Vector3 respawnStartPosition;
        private Vector3 respawnTargetPosition;

        public bool IsInvulnerable => isRespawning && invulnerableDuringRespawn;

        private void Reset()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<EnemyMeleeDamageHitbox>();
            if (navigationAgent == null) navigationAgent = GetComponent<NavMeshAgent>();
            if (bodyCollider == null) bodyCollider = GetComponent<CapsuleCollider>();
            if (physicsBody == null) physicsBody = GetComponent<Rigidbody>();
            ConfigureSolidBody();
        }

        private void Awake()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (playerTarget == null) playerTarget = FindPlayerTarget();
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<EnemyMeleeDamageHitbox>();
            if (navigationAgent == null) navigationAgent = GetComponent<NavMeshAgent>();
            if (bodyCollider == null) bodyCollider = GetComponent<CapsuleCollider>();
            if (physicsBody == null) physicsBody = GetComponent<Rigidbody>();
            lastPosition = transform.position;

            if (configureSolidBodyOnAwake)
            {
                ConfigureSolidBody();
            }

            if (navigationAgent != null)
            {
                navigationAgent.stoppingDistance = stopDistance;
                navigationAgent.updateRotation = false;
                navigationAgent.radius = colliderRadius;
                navigationAgent.height = colliderHeight;
                navigationAgent.avoidancePriority = avoidancePriority;
                navigationAgent.obstacleAvoidanceType = obstacleAvoidance;
            }
        }

        private void OnEnable()
        {
            if (playRespawnSequenceOnEnable)
            {
                BeginRespawnSequence();
            }
        }

        private void Update()
        {
            if (skeletonAnimator == null) return;
            if (animationBridge != null && animationBridge.IsDying)
            {
                SetMoving(false);
                StopNavigation();
                return;
            }

            if (isRespawning)
            {
                UpdateRespawnSequence();
                UpdateMovingFromTransformDelta();
                return;
            }

            if (playerTarget == null) playerTarget = FindPlayerTarget();
            if (playerTarget == null)
            {
                skeletonAnimator.SetBool(IsPlayerNearHash, false);
                SetMoving(false);
                return;
            }

            attackCooldownTimer -= Time.deltaTime;
            bool isPlayerNear = IsPlayerInsideProximityRange();

            if (!isPlayerNear && useVision)
            {
                isPlayerNear = CanSeePlayer();
            }

            if (!isPlayerNear && useHearing)
            {
                isPlayerNear = HasRecentHearingContact();
            }

            skeletonAnimator.SetBool(IsPlayerNearHash, isPlayerNear);

            if (animationBridge != null && animationBridge.IsAttacking)
            {
                UpdateAttackState();
                SetMoving(false);
                return;
            }

            if (isPlayerNear)
            {
                UpdatePlayerEngagement();
            }
            else
            {
                UpdatePatrol();
            }

            UpdateMovingFromTransformDelta();
        }

        public void RegisterNoise(Vector3 noisePosition, float noiseRadius)
        {
            if (!useHearing || playerTarget == null) return;

            float effectiveRadius = Mathf.Max(hearingRange, noiseRadius);
            if (Vector3.Distance(transform.position, noisePosition) <= effectiveRadius)
            {
                lastHeardTime = Time.time;
            }
        }

        private bool IsPlayerInsideProximityRange()
        {
            return Vector3.Distance(transform.position, playerTarget.position) <= proximityRange;
        }

        private bool CanSeePlayer()
        {
            Vector3 toPlayer = playerTarget.position - transform.position;
            if (toPlayer.sqrMagnitude > visionRange * visionRange) return false;

            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 flatToPlayer = Vector3.ProjectOnPlane(toPlayer, Vector3.up).normalized;
            float angleToPlayer = Vector3.Angle(flatForward, flatToPlayer);
            if (angleToPlayer > visionAngle * 0.5f) return false;

            Vector3 origin = transform.position + Vector3.up * 1.6f;
            Vector3 target = playerTarget.position + Vector3.up * 1.2f;
            Vector3 direction = target - origin;
            float distance = direction.magnitude;
            if (distance <= 0.01f) return true;

            if (Physics.Raycast(origin, direction.normalized, out var hit, distance, visionBlockers, QueryTriggerInteraction.Ignore))
            {
                return hit.transform == playerTarget || hit.transform.IsChildOf(playerTarget);
            }

            return true;
        }

        private bool HasRecentHearingContact()
        {
            return Time.time - lastHeardTime <= hearingMemoryDuration;
        }

        private void UpdatePlayerEngagement()
        {
            float distanceToPlayer = HorizontalDistanceTo(playerTarget.position);

            if (rotateTowardPlayerInRange)
            {
                RotateTowardPlayer();
            }

            if (canAttack && distanceToPlayer <= attackRange)
            {
                StopNavigation();
                SetMoving(false);
                TryAttack();
                return;
            }

            if (moveTowardPlayerInRange)
            {
                MoveTowardPlayer();
                return;
            }

            StopNavigation();
            SetMoving(false);
        }

        private void UpdatePatrol()
        {
            if (!usePatrol || patrolPoints == null || patrolPoints.Length == 0)
            {
                StopNavigation();
                SetMoving(false);
                return;
            }

            Transform patrolPoint = patrolPoints[patrolIndex];
            if (patrolPoint == null)
            {
                AdvancePatrolPoint();
                SetMoving(false);
                return;
            }

            float distanceToPatrol = HorizontalDistanceTo(patrolPoint.position);
            if (distanceToPatrol <= stopDistance)
            {
                StopNavigation();
                SetMoving(false);
                patrolWaitTimer += Time.deltaTime;
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    patrolWaitTimer = 0f;
                    AdvancePatrolPoint();
                }

                return;
            }

            patrolWaitTimer = 0f;
            MoveTowardsPosition(patrolPoint.position, false);
        }

        private void RotateTowardPlayer()
        {
            Vector3 direction = playerTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            float yawOffset = additionalFacingYawOffset + (modelForwardIsReversed ? 180f : 0f);
            targetRotation *= Quaternion.Euler(0f, yawOffset, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        private void MoveTowardPlayer()
        {
            MoveTowardsPosition(playerTarget.position, true);
        }

        private void MoveTowardsPosition(Vector3 targetPosition, bool stopAtAttackDistance)
        {
            float desiredStopDistance = stopAtAttackDistance && canAttack ? Mathf.Max(stopDistance, attackRange) : stopDistance;
            Vector3 offset = targetPosition - transform.position;
            offset.y = 0f;

            float distance = offset.magnitude;
            if (distance <= desiredStopDistance || distance <= 0.0001f)
            {
                StopNavigation();
                SetMoving(false);
                return;
            }

            if (ShouldUseNavMeshMovement())
            {
                navigationAgent.stoppingDistance = desiredStopDistance;
                if (HasNavigablePath(targetPosition))
                {
                    navigationAgent.SetDestination(targetPosition);
                    SetMoving(true);
                    return;
                }
                else
                {
                    StopNavigation();
                    if (!allowDirectMovementFallback)
                    {
                        SetMoving(false);
                        return;
                    }
                }
            }

            if (!allowDirectMovementFallback && ShouldUseNavMeshMovement())
            {
                SetMoving(false);
                return;
            }

            Vector3 moveDirection = offset / distance;
            TryMoveDirect(moveDirection, targetPosition);
        }

        private void TryAttack()
        {
            if (!canAttack || attackCooldownTimer > 0f) return;

            attackCooldownTimer = attackCooldown;
            attackTimer = fallbackAttackDuration;
            animationBridge?.BeginAttack();
        }

        private void UpdateAttackState()
        {
            attackTimer -= Time.deltaTime;
            bool animationFinished = animationBridge != null && animationBridge.IsCurrentAnimationFinished();
            if (animationFinished || (useFallbackAttackTimer && attackTimer <= 0f))
            {
                EndAttack();
            }
        }

        private void EndAttack()
        {
            meleeHitbox?.EndAttackWindow();
            animationBridge?.EndAttack();
        }

        public void AnimationEvent_BeginAttackWindow()
        {
            meleeHitbox?.BeginAttackWindow();
        }

        public void AnimationEvent_EndAttackWindow()
        {
            meleeHitbox?.EndAttackWindow();
        }

        public void AnimationEvent_EndAttack()
        {
            EndAttack();
        }

        public void AnimationEvent_EndRespawn()
        {
            EndRespawnSequence();
        }

        private bool ShouldUseNavMeshMovement()
        {
            return useNavMeshIfAvailable
                   && navigationAgent != null
                   && navigationAgent.enabled
                   && navigationAgent.isOnNavMesh;
        }

        private bool HasNavigablePath(Vector3 targetPosition)
        {
            if (!ShouldUseNavMeshMovement()) return false;

            NavMeshPath path = new();
            if (!navigationAgent.CalculatePath(targetPosition, path)) return false;
            if (path.corners == null || path.corners.Length == 0) return false;
            return path.status == requiredPathStatus;
        }

        private void StopNavigation()
        {
            if (!ShouldUseNavMeshMovement()) return;
            navigationAgent.ResetPath();
        }

        private void SetMoving(bool isMoving)
        {
            animationBridge?.SetMoving(isMoving);
        }

        private void UpdateMovingFromTransformDelta()
        {
            float movement = (transform.position - lastPosition).magnitude;
            if (animationBridge != null && !animationBridge.IsAttacking && !animationBridge.IsDying)
            {
                animationBridge.SetMoving(movement > 0.001f);
            }

            lastPosition = transform.position;
        }

        private float HorizontalDistanceTo(Vector3 position)
        {
            Vector3 offset = position - transform.position;
            offset.y = 0f;
            return offset.magnitude;
        }

        private void AdvancePatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        public void BeginRespawnSequence()
        {
            if (animationBridge != null && animationBridge.IsDying) return;

            isRespawning = true;
            respawnTimer = Mathf.Max(0.01f, respawnSequenceDuration);
            respawnTargetPosition = transform.position;
            respawnStartPosition = applyRespawnMovementOffset ? transform.position + transform.rotation * respawnMovementOffset : transform.position;

            if (applyRespawnMovementOffset)
            {
                if (ShouldUseNavMeshMovement())
                {
                    navigationAgent.Warp(respawnStartPosition);
                }
                else
                {
                    transform.position = respawnStartPosition;
                }
            }

            meleeHitbox?.EndAttackWindow();
            StopNavigation();
            SetMoving(false);
            animationBridge?.BeginRespawn();
        }

        private void UpdateRespawnSequence()
        {
            StopNavigation();
            SetMoving(false);

            if (applyRespawnMovementOffset)
            {
                float progress = 1f - Mathf.Clamp01(respawnTimer / Mathf.Max(0.01f, respawnSequenceDuration));
                float curveValue = respawnMovementCurve != null ? respawnMovementCurve.Evaluate(progress) : progress;
                Vector3 position = Vector3.LerpUnclamped(respawnStartPosition, respawnTargetPosition, curveValue);
                if (ShouldUseNavMeshMovement())
                {
                    navigationAgent.Warp(position);
                }
                else
                {
                    transform.position = position;
                }
            }

            if (!useFallbackRespawnTimer) return;

            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0f)
            {
                EndRespawnSequence();
            }
        }

        private void EndRespawnSequence()
        {
            if (!isRespawning) return;

            isRespawning = false;
            animationBridge?.EndRespawn();
            if (applyRespawnMovementOffset)
            {
                if (ShouldUseNavMeshMovement())
                {
                    navigationAgent.Warp(respawnTargetPosition);
                }
                else
                {
                    transform.position = respawnTargetPosition;
                }
            }
        }

        private void ConfigureSolidBody()
        {
            if (bodyCollider != null)
            {
                bodyCollider.isTrigger = false;
                bodyCollider.center = colliderCenter;
                bodyCollider.height = colliderHeight;
                bodyCollider.radius = colliderRadius;
            }

            if (physicsBody == null) return;
            physicsBody.isKinematic = true;
            physicsBody.useGravity = false;
            physicsBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void TryMoveDirect(Vector3 desiredDirection, Vector3 targetPosition)
        {
            Vector3 movement = ResolveDirectMoveDirection(desiredDirection, targetPosition);
            if (movement.sqrMagnitude <= 0.0001f)
            {
                SetMoving(false);
                return;
            }

            transform.position += movement * (moveSpeed * Time.deltaTime);
            SetMoving(true);
        }

        private Vector3 ResolveDirectMoveDirection(Vector3 desiredDirection, Vector3 targetPosition)
        {
            if (!IsDirectionBlocked(desiredDirection)) return desiredDirection;

            float bestScore = float.NegativeInfinity;
            Vector3 bestDirection = Vector3.zero;

            for (int i = 1; i <= directMoveAvoidanceChecksPerSide; i++)
            {
                float angle = directMoveAvoidanceAngle * i;
                Vector3 left = Quaternion.Euler(0f, -angle, 0f) * desiredDirection;
                EvaluateDirectMoveCandidate(left, targetPosition, ref bestDirection, ref bestScore);

                Vector3 right = Quaternion.Euler(0f, angle, 0f) * desiredDirection;
                EvaluateDirectMoveCandidate(right, targetPosition, ref bestDirection, ref bestScore);
            }

            return bestDirection;
        }

        private void EvaluateDirectMoveCandidate(Vector3 candidateDirection, Vector3 targetPosition, ref Vector3 bestDirection, ref float bestScore)
        {
            if (candidateDirection.sqrMagnitude <= 0.0001f || IsDirectionBlocked(candidateDirection)) return;

            Vector3 nextPosition = transform.position + candidateDirection.normalized * moveSpeed * Time.deltaTime;
            float score = Vector3.Dot(candidateDirection.normalized, (targetPosition - nextPosition).normalized);
            if (score > bestScore)
            {
                bestScore = score;
                bestDirection = candidateDirection.normalized;
            }
        }

        private bool IsDirectionBlocked(Vector3 direction)
        {
            Vector3 castDirection = direction.normalized;
            if (castDirection.sqrMagnitude <= 0.0001f) return true;

            float castDistance = moveSpeed * Time.deltaTime + directMoveCastPadding;
            GetColliderCapsule(out Vector3 pointOne, out Vector3 pointTwo, out float radius);
            RaycastHit[] hits = Physics.CapsuleCastAll(pointOne, pointTwo, radius, castDirection, castDistance, movementBlockers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                Transform hitTransform = hits[i].transform;
                if (hitTransform == null || hitTransform == transform || hitTransform.IsChildOf(transform)) continue;
                return true;
            }

            return false;
        }

        private void GetColliderCapsule(out Vector3 pointOne, out Vector3 pointTwo, out float radius)
        {
            Vector3 center = bodyCollider != null
                ? transform.TransformPoint(bodyCollider.center)
                : transform.position + Vector3.up * colliderCenter.y;

            float height = bodyCollider != null ? bodyCollider.height : colliderHeight;
            radius = bodyCollider != null ? bodyCollider.radius : colliderRadius;
            float halfHeight = Mathf.Max(radius, height * 0.5f) - radius;
            Vector3 up = transform.up * halfHeight;
            Vector3 step = Vector3.up * directMoveStepHeight;
            pointOne = center + up + step;
            pointTwo = center - up + step;
        }

        private static Transform FindPlayerTarget()
        {
            GameObject player = GameObject.Find("Player");
            return player != null ? player.transform : null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, proximityRange);

            if (canAttack)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }

            if (moveTowardPlayerInRange)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, stopDistance);
            }

            if (useVision)
            {
                Vector3 origin = transform.position;
                Vector3 leftBoundary = Quaternion.Euler(0f, -visionAngle * 0.5f, 0f) * transform.forward * visionRange;
                Vector3 rightBoundary = Quaternion.Euler(0f, visionAngle * 0.5f, 0f) * transform.forward * visionRange;

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(origin, visionRange);
                Gizmos.DrawLine(origin, origin + leftBoundary);
                Gizmos.DrawLine(origin, origin + rightBoundary);
            }

            if (useHearing)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, hearingRange);
            }

            if (usePatrol && patrolPoints != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    Transform patrolPoint = patrolPoints[i];
                    if (patrolPoint == null) continue;

                    Gizmos.DrawWireSphere(patrolPoint.position, 0.2f);
                    if (i + 1 < patrolPoints.Length && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoint.position, patrolPoints[i + 1].position);
                    }
                }
            }
        }
    }
}
