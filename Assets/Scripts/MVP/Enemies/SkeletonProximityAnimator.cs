using TinyHunter.Core.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace TinyHunter.MVP.Enemies
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class SkeletonProximityAnimator : MonoBehaviour, ITemporaryInvulnerabilityProvider
    {
        private static readonly int IsPlayerNearHash = Animator.StringToHash("IsPlayerNear");

        [Header("References")]
        [SerializeField] private Animator skeletonAnimator;
        [SerializeField] private Transform playerTarget;
        [SerializeField] private EnemyAnimationBridge animationBridge;
        [SerializeField] private EnemyMeleeDamageHitbox meleeHitbox;
        [SerializeField] private NavMeshAgent navigationAgent;
        [SerializeField] private CapsuleCollider bodyCollider;

        [Header("Detection")]
        [SerializeField] private float proximityRange = 4f;
        [SerializeField] private float attackRange = 1.6f;
        [SerializeField] private float attackCooldown = 1.25f;
        [SerializeField] private float attackFallbackDuration = 0.9f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 1.5f;
        [SerializeField] private float stopDistance = 1.2f;
        [SerializeField] private bool useNavMesh = true;
        [SerializeField] private float navMeshSnapDistance = 1f;
        [SerializeField] private LayerMask directMoveBlockers = Physics.DefaultRaycastLayers;
        [SerializeField] private float directMoveCastPadding = 0.05f;
        [SerializeField] private bool rotateTowardTarget = true;
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private bool modelForwardIsReversed = true;
        [SerializeField] private float additionalFacingYawOffset;

        [Header("Attack Routing")]
        [SerializeField] private bool requireClearAttackPath = true;
        [SerializeField] private LayerMask attackBlockers = Physics.DefaultRaycastLayers;
        [SerializeField] private float attackPathHeight = 1f;
        [SerializeField] private float surroundOffset = 0.8f;
        [SerializeField] private float surroundRadiusPadding = 0.15f;
        [SerializeField] private float crowdCheckRadius = 0.35f;

        [Header("Patrol")]
        [SerializeField] private bool usePatrol;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 1.5f;

        [Header("Respawn")]
        [SerializeField] private bool playRespawnSequenceOnEnable = true;
        [SerializeField] private float respawnDuration = 1.1f;
        [SerializeField] private bool invulnerableDuringRespawn = true;
        [SerializeField] private Vector3 respawnOffset = new(0f, 0f, -0.35f);

        [Header("Body")]
        [SerializeField] private Vector3 colliderCenter = new(0f, 0.9f, 0f);
        [SerializeField] private float colliderHeight = 1.8f;
        [SerializeField] private float colliderRadius = 0.35f;

        private float attackCooldownTimer;
        private float attackTimer;
        private float patrolWaitTimer;
        private int patrolIndex;
        private bool isRespawning;
        private float respawnTimer;
        private Vector3 respawnStartPosition;
        private Vector3 respawnTargetPosition;

        public bool IsInvulnerable => isRespawning && invulnerableDuringRespawn;

        private void Reset()
        {
            CacheReferences();
            SyncBodyAndAgent();
            EnsureOnNavMesh();
        }

        private void Awake()
        {
            CacheReferences();
            SyncBodyAndAgent();
        }

        private void OnEnable()
        {
            EnsureOnNavMesh();
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
                StopMoving();
                return;
            }

            if (isRespawning)
            {
                UpdateRespawnSequence();
                return;
            }

            if (playerTarget == null)
            {
                playerTarget = FindPlayerTarget();
            }

            attackCooldownTimer -= Time.deltaTime;

            if (playerTarget == null)
            {
                skeletonAnimator.SetBool(IsPlayerNearHash, false);
                UpdatePatrol();
                return;
            }

            float distanceToPlayer = HorizontalDistanceTo(playerTarget.position);
            bool isPlayerNear = distanceToPlayer <= proximityRange;
            skeletonAnimator.SetBool(IsPlayerNearHash, isPlayerNear);

            if (animationBridge != null && animationBridge.IsAttacking)
            {
                UpdateAttackState();
                return;
            }

            if (isPlayerNear)
            {
                EngagePlayer(distanceToPlayer);
            }
            else
            {
                UpdatePatrol();
            }
        }

        public void BeginRespawnSequence()
        {
            if (animationBridge != null && animationBridge.IsDying) return;

            isRespawning = true;
            respawnTimer = Mathf.Max(0.01f, respawnDuration);
            respawnTargetPosition = transform.position;
            respawnStartPosition = transform.position + transform.rotation * respawnOffset;
            WarpTo(respawnStartPosition);
            StopMoving();
            meleeHitbox?.EndAttackWindow();
            animationBridge?.BeginRespawn();
        }

        public void AnimationEvent_EndRespawn()
        {
            EndRespawnSequence();
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

        private void EngagePlayer(float distanceToPlayer)
        {
            if (rotateTowardTarget)
            {
                RotateToward(playerTarget.position);
            }

            bool clearAttackPath = HasClearAttackPath();
            bool crowded = IsCrowdedAtAttackPosition();
            bool forceAttackAtCloseRange = distanceToPlayer <= Mathf.Max(0.75f, attackRange * 0.6f);
            if (distanceToPlayer <= attackRange && clearAttackPath && (!crowded || forceAttackAtCloseRange))
            {
                StopMoving();
                TryAttack();
                return;
            }

            if (!clearAttackPath || crowded)
            {
                float approachStopDistance = Mathf.Max(0.05f, Mathf.Min(stopDistance, attackRange) * 0.35f);
                MoveTo(GetApproachPosition(), approachStopDistance);
                return;
            }

            MoveTo(playerTarget.position, Mathf.Max(stopDistance, attackRange));
        }

        private void UpdatePatrol()
        {
            if (!usePatrol || patrolPoints == null || patrolPoints.Length == 0)
            {
                StopMoving();
                return;
            }

            Transform patrolPoint = patrolPoints[patrolIndex];
            if (patrolPoint == null)
            {
                AdvancePatrolPoint();
                StopMoving();
                return;
            }

            float distanceToPatrol = HorizontalDistanceTo(patrolPoint.position);
            if (distanceToPatrol <= stopDistance)
            {
                StopMoving();
                patrolWaitTimer += Time.deltaTime;
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    patrolWaitTimer = 0f;
                    AdvancePatrolPoint();
                }

                return;
            }

            patrolWaitTimer = 0f;
            MoveTo(patrolPoint.position, stopDistance);
        }



        private bool IsCrowdedAtAttackPosition()
        {
            if (playerTarget == null) return false;

            Vector3 toPlayer = playerTarget.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude <= 0.001f) return false;

            Vector3 checkCenter = transform.position + toPlayer.normalized * Mathf.Max(0.2f, attackRange * 0.7f);
            Collider[] hits = Physics.OverlapSphere(checkCenter, crowdCheckRadius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                Transform t = hits[i].transform;
                if (t == null || t == transform || t.IsChildOf(transform)) continue;
                if (t == playerTarget || t.IsChildOf(playerTarget)) continue;
                if (t.gameObject.layer == gameObject.layer)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasClearAttackPath()
        {
            if (!requireClearAttackPath || playerTarget == null) return true;

            Vector3 origin = transform.position + Vector3.up * attackPathHeight;
            Vector3 target = playerTarget.position + Vector3.up * attackPathHeight;
            Vector3 direction = target - origin;
            float distance = direction.magnitude;
            if (distance <= 0.01f) return true;

            RaycastHit[] hits = Physics.RaycastAll(origin, direction.normalized, distance, attackBlockers, QueryTriggerInteraction.Ignore);
            if (hits == null || hits.Length == 0) return true;

            float closestDistance = float.MaxValue;
            Transform closestHit = null;
            for (int i = 0; i < hits.Length; i++)
            {
                Transform hitTransform = hits[i].transform;
                if (hitTransform == null) continue;
                if (hitTransform == transform || hitTransform.IsChildOf(transform)) continue;

                if (hits[i].distance < closestDistance)
                {
                    closestDistance = hits[i].distance;
                    closestHit = hitTransform;
                }
            }

            if (closestHit == null) return true;
            return closestHit == playerTarget || closestHit.IsChildOf(playerTarget);
        }

        private Vector3 GetApproachPosition()
        {
            if (playerTarget == null) return transform.position;

            float desiredRadius = Mathf.Max(0.1f, attackRange - surroundRadiusPadding);
            Vector3 radial = transform.position - playerTarget.position;
            radial.y = 0f;
            if (radial.sqrMagnitude <= 0.001f)
            {
                radial = -playerTarget.forward;
                radial.y = 0f;
            }

            radial = radial.normalized;
            float sideSign = (Mathf.Abs(GetInstanceID()) & 1) == 0 ? 1f : -1f;
            float sidestepAngle = 50f * sideSign;
            Quaternion spreadRotation = Quaternion.Euler(0f, sidestepAngle, 0f);
            Vector3 spreadDir = (spreadRotation * radial).normalized;
            Vector3 candidate = playerTarget.position + spreadDir * desiredRadius + Vector3.Cross(Vector3.up, radial).normalized * (surroundOffset * sideSign);
            candidate.y = transform.position.y;

            if (useNavMesh && NavMesh.SamplePosition(candidate, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
            {
                return navHit.position;
            }

            return candidate;
        }

        private void MoveTo(Vector3 targetPosition, float desiredStopDistance)
        {
            Vector3 offset = targetPosition - transform.position;
            offset.y = 0f;

            float distance = offset.magnitude;
            if (distance <= desiredStopDistance || distance <= 0.001f)
            {
                StopMoving();
                return;
            }

            if (useNavMesh && CanUseNavMesh())
            {
                navigationAgent.speed = moveSpeed;
                navigationAgent.stoppingDistance = desiredStopDistance;
                navigationAgent.SetDestination(targetPosition);
                animationBridge?.SetMoving(true);
                return;
            }

            Vector3 direction = offset / distance;
            if (IsDirectPathBlocked(direction))
            {
                StopMoving();
                return;
            }

            transform.position += direction * (moveSpeed * Time.deltaTime);
            animationBridge?.SetMoving(true);
        }

        private void StopMoving()
        {
            if (CanUseNavMesh())
            {
                navigationAgent.ResetPath();
                navigationAgent.velocity = Vector3.zero;
            }

            animationBridge?.SetMoving(false);
        }

        private void TryAttack()
        {
            if (attackCooldownTimer > 0f || animationBridge == null) return;

            attackCooldownTimer = attackCooldown;
            attackTimer = attackFallbackDuration;
            animationBridge.BeginAttack();
        }

        private void UpdateAttackState()
        {
            attackTimer -= Time.deltaTime;
            bool animationFinished = animationBridge != null && animationBridge.IsCurrentAnimationFinished();
            if (animationFinished || attackTimer <= 0f)
            {
                EndAttack();
            }
        }

        private void EndAttack()
        {
            meleeHitbox?.EndAttackWindow();
            animationBridge?.EndAttack();
        }

        private void UpdateRespawnSequence()
        {
            StopMoving();
            respawnTimer -= Time.deltaTime;

            float progress = 1f - Mathf.Clamp01(respawnTimer / Mathf.Max(0.01f, respawnDuration));
            Vector3 position = Vector3.Lerp(respawnStartPosition, respawnTargetPosition, progress);
            WarpTo(position);

            if (respawnTimer <= 0f)
            {
                EndRespawnSequence();
            }
        }

        private void EndRespawnSequence()
        {
            if (!isRespawning) return;

            isRespawning = false;
            WarpTo(respawnTargetPosition);
            animationBridge?.EndRespawn();
        }

        private void RotateToward(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f) return;

            float yawOffset = additionalFacingYawOffset + (modelForwardIsReversed ? 180f : 0f);
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up) * Quaternion.Euler(0f, yawOffset, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        private bool IsDirectPathBlocked(Vector3 direction)
        {
            Vector3 center = transform.TransformPoint(bodyCollider != null ? bodyCollider.center : colliderCenter);
            float radius = bodyCollider != null ? bodyCollider.radius : colliderRadius;
            float height = bodyCollider != null ? bodyCollider.height : colliderHeight;
            float halfHeight = Mathf.Max(radius, height * 0.5f) - radius;
            Vector3 top = center + Vector3.up * halfHeight;
            Vector3 bottom = center - Vector3.up * halfHeight;
            float castDistance = moveSpeed * Time.deltaTime + directMoveCastPadding;

            return Physics.CapsuleCast(top, bottom, radius, direction.normalized, out _, castDistance, directMoveBlockers, QueryTriggerInteraction.Ignore);
        }

        private float HorizontalDistanceTo(Vector3 targetPosition)
        {
            Vector3 offset = targetPosition - transform.position;
            offset.y = 0f;
            return offset.magnitude;
        }


        private void EnsureOnNavMesh()
        {
            if (!useNavMesh || navigationAgent == null || !navigationAgent.enabled || navigationAgent.isOnNavMesh) return;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSnapDistance, NavMesh.AllAreas))
            {
                navigationAgent.Warp(hit.position);
            }
        }

        private bool CanUseNavMesh()
        {
            return navigationAgent != null && navigationAgent.enabled && navigationAgent.isOnNavMesh;
        }

        private void WarpTo(Vector3 position)
        {
            if (CanUseNavMesh())
            {
                navigationAgent.Warp(position);
                return;
            }

            transform.position = position;
        }

        private void AdvancePatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0) return;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        private void CacheReferences()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (playerTarget == null) playerTarget = FindPlayerTarget();
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<EnemyMeleeDamageHitbox>();
            if (navigationAgent == null) navigationAgent = GetComponent<NavMeshAgent>();
            if (bodyCollider == null) bodyCollider = GetComponent<CapsuleCollider>();
        }

        private void SyncBodyAndAgent()
        {
            if (bodyCollider != null)
            {
                bodyCollider.isTrigger = false;
                bodyCollider.center = colliderCenter;
                bodyCollider.height = colliderHeight;
                bodyCollider.radius = colliderRadius;
            }

            if (navigationAgent != null)
            {
                navigationAgent.updateRotation = false;
                navigationAgent.speed = moveSpeed;
                navigationAgent.radius = colliderRadius;
                navigationAgent.height = colliderHeight;
                navigationAgent.stoppingDistance = stopDistance;
                navigationAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                navigationAgent.avoidancePriority = 20 + Mathf.Abs(GetInstanceID() % 60);
            }
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

            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopDistance);

            if (!usePatrol || patrolPoints == null) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Transform point = patrolPoints[i];
                if (point == null) continue;

                Gizmos.DrawWireSphere(point.position, 0.2f);
                Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
                if (nextPoint != null)
                {
                    Gizmos.DrawLine(point.position, nextPoint.position);
                }
            }
        }
    }
}
