using TinyHunter.Core.Equipment;
using TinyHunter.Core.Input;
using TinyHunter.Core.Player;
using TinyHunter.Data.Combat;
using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class WeaponCombatSystem : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerAnimationBridge animationBridge;
        [SerializeField] private EquipmentSystem equipmentSystem;
        [SerializeField] private WeaponDefinition fallbackWeapon;
        [SerializeField] private float attackRadius = 0.9f;
        [SerializeField] private float attackStateDuration = 0.25f;
        [SerializeField] private LayerMask hittableMask;

        private WeaponDefinition EquippedWeapon => equipmentSystem != null && equipmentSystem.EquippedWeapon != null
            ? equipmentSystem.EquippedWeapon
            : fallbackWeapon;

        private void Update()
        {
            var weapon = EquippedWeapon;
            if (weapon == null || playerStats == null || input == null || playerController == null || playerStats.IsDead) return;

            bool canGuard = weapon.CanBlock && playerController.CanAct();
            playerStats.SetBlocking(canGuard && input.GuardHeld, weapon.GuardDamageReduction);

            if (!playerController.CanAct()) return;

            if (input.PrimaryAttackPressed)
            {
                TryAttack(AttackWeight.Light);
            }
        }

        private void TryAttack(AttackWeight weight)
        {
            var weapon = EquippedWeapon;
            if (weapon == null) return;

            float staminaCost = weight == AttackWeight.Light ? weapon.StaminaCostLight : weapon.StaminaCostHeavy;
            if (!playerStats.SpendStamina(staminaCost)) return;

            playerController.SetAttackState(attackStateDuration);
            if (weight == AttackWeight.Light) animationBridge?.TriggerLightAttack();
            else animationBridge?.TriggerHeavyAttack();

            float damageMultiplier = weight == AttackWeight.Light ? 1f : 1.8f;
            float damage = weapon.AttackPower * damageMultiplier;

            var hits = Physics.OverlapSphere(transform.position + transform.forward * weapon.Reach, attackRadius, hittableMask);
            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<IDamageable>(out var target)) continue;

                string partId = null;
                if (hit.TryGetComponent<MonsterPartHitbox>(out var partHitbox))
                {
                    partId = partHitbox.PartId;
                }

                target.TakeHit(damage, weapon.DamageType, partId, hit.ClosestPoint(transform.position));
                HitFeedbackSystem.Instance?.PlayHitFeedback();
            }
        }
    }
}
