using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class TargetLockSystem : MonoBehaviour
    {
        [SerializeField] private Transform owner;
        [SerializeField] private float lockRadius = 12f;
        [SerializeField] private LayerMask targetMask;
        public Transform CurrentTarget { get; private set; }

        public void ToggleLock()
        {
            if (CurrentTarget != null)
            {
                CurrentTarget = null;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(owner != null ? owner.position : transform.position, lockRadius, targetMask);
            float bestDistance = float.MaxValue;
            Transform bestTarget = null;

            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestTarget = hit.transform;
                }
            }

            CurrentTarget = bestTarget;
        }
    }
}
