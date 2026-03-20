using System.Collections.Generic;
using TinyHunter.Core.Combat;
using UnityEngine;

namespace TinyHunter.MVP.Enemies
{
    public class EnemyMeleeDamageHitbox : MonoBehaviour
    {
        [SerializeField] private Collider hitboxCollider;
        [SerializeField] private float damage = 12f;
        [SerializeField] private LayerMask targetMask = ~0;

        private readonly HashSet<int> hitTargetsThisSwing = new();
        private bool attackWindowActive;

        private void Reset()
        {
            if (hitboxCollider == null) hitboxCollider = GetComponent<Collider>();
        }

        private void Awake()
        {
            if (hitboxCollider == null) hitboxCollider = GetComponent<Collider>();
            if (hitboxCollider != null) hitboxCollider.enabled = false;
        }

        public void BeginAttackWindow()
        {
            attackWindowActive = true;
            hitTargetsThisSwing.Clear();
            if (hitboxCollider != null) hitboxCollider.enabled = true;
        }

        public void EndAttackWindow()
        {
            attackWindowActive = false;
            hitTargetsThisSwing.Clear();
            if (hitboxCollider != null) hitboxCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!attackWindowActive) return;
            if (((1 << other.gameObject.layer) & targetMask.value) == 0) return;

            PlayerStats stats = other.GetComponentInParent<PlayerStats>();
            if (stats == null || stats.IsDead) return;

            int targetId = stats.gameObject.GetInstanceID();
            if (!hitTargetsThisSwing.Add(targetId)) return;

            stats.ApplyDamage(damage);
        }
    }
}
