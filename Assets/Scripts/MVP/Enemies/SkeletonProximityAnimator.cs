using UnityEngine;

namespace TinyHunter.MVP.Enemies
{
    public class SkeletonProximityAnimator : MonoBehaviour
    {
        private static readonly int IsPlayerNearHash = Animator.StringToHash("IsPlayerNear");

        [Header("Required References")]
        [SerializeField] private Animator skeletonAnimator;
        [SerializeField] private Transform playerTarget;

        [Header("Current MVP Detection")]
        [SerializeField] private float proximityRange = 4f;
        [SerializeField] private bool rotateTowardPlayerInRange = true;
        [SerializeField] private float turnSpeed = 360f;
        [SerializeField] private bool modelForwardIsReversed = true;

        [Header("Future Detection - Disabled By Default")]
        [SerializeField] private bool useVision;
        [SerializeField] private float visionRange = 8f;
        [SerializeField, Range(1f, 180f)] private float visionAngle = 75f;
        [SerializeField] private LayerMask visionBlockers = Physics.DefaultRaycastLayers;

        [SerializeField] private bool useHearing;
        [SerializeField] private float hearingRange = 6f;
        [SerializeField] private float hearingMemoryDuration = 1.5f;

        private float lastHeardTime = float.NegativeInfinity;

        private void Reset()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
        }

        private void Awake()
        {
            if (skeletonAnimator == null) skeletonAnimator = GetComponentInChildren<Animator>();
            if (playerTarget == null) playerTarget = FindPlayerTarget();
        }

        private void Update()
        {
            if (skeletonAnimator == null) return;
            if (playerTarget == null) playerTarget = FindPlayerTarget();
            if (playerTarget == null)
            {
                skeletonAnimator.SetBool(IsPlayerNearHash, false);
                return;
            }

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

            if (isPlayerNear && rotateTowardPlayerInRange)
            {
                RotateTowardPlayer();
            }
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

        private void RotateTowardPlayer()
        {
            Vector3 direction = playerTarget.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f) return;
            if (modelForwardIsReversed) direction = -direction;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
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
        }
    }
}
