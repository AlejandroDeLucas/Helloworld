using UnityEngine;
using UnityEngine.AI;

namespace TinyHunter.MVP.Enemies
{
    public class SkeletonProximityAnimator : MonoBehaviour
    {
        private static readonly int IsPlayerNearHash = Animator.StringToHash("IsPlayerNear");

        [Header("Required References")]
        [SerializeField] private Animator skeletonAnimator;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private EnemyAnimationBridge animationBridge;
        [SerializeField] private EnemyMeleeDamageHitbox meleeHitbox;
        [SerializeField] private NavMeshAgent navigationAgent;

        [Header("Current MVP Detection")]
        [SerializeField] private float proximityRange = 4f;
        [SerializeField] private bool rotateTowardPlayerInRange = true;
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private bool modelForwardIsReversed = true;
        [SerializeField] private bool moveTowardPlayerInRange = true;
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float stopDistance = 1.2f;
        [SerializeField] private bool useNavMeshIfAvailable = true;

        [Header("Combat")]
        [SerializeField] private bool canAttack = true;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float attackCooldown = 1.25f;
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

        private void Reset()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<EnemyMeleeDamageHitbox>();
            if (navigationAgent == null) navigationAgent = GetComponent<NavMeshAgent>();
        }

        private void Awake()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (playerTarget == null) playerTarget = FindPlayerTarget();
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<EnemyMeleeDamageHitbox>();
            if (navigationAgent == null) navigationAgent = GetComponent<NavMeshAgent>();
            lastPosition = transform.position;

            if (navigationAgent != null)
            {
                navigationAgent.stoppingDistance = stopDistance;
                navigationAgent.updateRotation = false;
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
            if (modelForwardIsReversed) direction = -direction;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
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
                navigationAgent.SetDestination(targetPosition);
                SetMoving(true);
                return;
            }

            Vector3 moveDirection = offset / distance;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
            SetMoving(true);
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
            if (attackTimer <= 0f)
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

        private bool ShouldUseNavMeshMovement()
        {
            return useNavMeshIfAvailable
                   && navigationAgent != null
                   && navigationAgent.enabled
                   && navigationAgent.isOnNavMesh;
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
