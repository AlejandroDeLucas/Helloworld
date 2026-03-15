using UnityEngine;

namespace TinyHunter.Core.Player
{
    public class PlayerAnimationBridge : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int LightAttackTrigger = Animator.StringToHash("LightAttack");
        private static readonly int HeavyAttackTrigger = Animator.StringToHash("HeavyAttack");
        private static readonly int DodgeTrigger = Animator.StringToHash("Dodge");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");
        private static readonly int MoveSpeedFloat = Animator.StringToHash("MoveSpeed");

        public void TriggerLightAttack() => SetTrigger(LightAttackTrigger);
        public void TriggerHeavyAttack() => SetTrigger(HeavyAttackTrigger);
        public void TriggerDodge() => SetTrigger(DodgeTrigger);
        public void TriggerHit() => SetTrigger(HitTrigger);
        public void TriggerDeath() => SetTrigger(DeathTrigger);

        public void SetMoveSpeed(float normalizedSpeed)
        {
            if (animator == null) return;
            animator.SetFloat(MoveSpeedFloat, Mathf.Clamp01(normalizedSpeed));
        }

        private void SetTrigger(int triggerHash)
        {
            if (animator == null) return;
            animator.SetTrigger(triggerHash);
        }
    }
}
