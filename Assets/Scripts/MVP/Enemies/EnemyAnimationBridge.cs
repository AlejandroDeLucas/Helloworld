using System.Collections;
using UnityEngine;

namespace TinyHunter.MVP.Enemies
{
    public class EnemyAnimationBridge : MonoBehaviour
    {
        private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        private static readonly int IsGettingHitHash = Animator.StringToHash("IsGettingHit");
        private static readonly int IsDyingHash = Animator.StringToHash("IsDying");

        [SerializeField] private Animator animator;
        [SerializeField] private bool useAnimatorRootMotion;
        [SerializeField] private float hitReactionDuration = 0.25f;
        [SerializeField] private bool canInterruptAttackWithHitReaction;

        private Coroutine hitReactionRoutine;

        public bool IsAttacking { get; private set; }
        public bool IsDying { get; private set; }

        private void Reset()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
            ApplyAnimatorSettings();
        }

        private void Awake()
        {
            ApplyAnimatorSettings();
        }

        public void SetMoving(bool isMoving)
        {
            if (animator == null || IsDying || IsAttacking) return;
            animator.SetBool(IsMovingHash, isMoving);
        }

        public void BeginAttack()
        {
            if (animator == null || IsDying) return;

            IsAttacking = true;
            animator.SetBool(IsMovingHash, false);
            animator.SetBool(IsAttackingHash, true);
        }

        public void EndAttack()
        {
            if (animator == null) return;

            IsAttacking = false;
            animator.SetBool(IsAttackingHash, false);
        }

        public void PlayHitReaction()
        {
            if (animator == null || IsDying) return;
            if (IsAttacking && !canInterruptAttackWithHitReaction) return;

            if (canInterruptAttackWithHitReaction)
            {
                EndAttack();
            }

            if (hitReactionRoutine != null)
            {
                StopCoroutine(hitReactionRoutine);
            }

            hitReactionRoutine = StartCoroutine(HitReactionRoutine());
        }

        public void SetDying()
        {
            if (animator == null) return;

            IsDying = true;
            IsAttacking = false;
            animator.SetBool(IsMovingHash, false);
            animator.SetBool(IsAttackingHash, false);
            animator.SetBool(IsGettingHitHash, false);
            animator.SetBool(IsDyingHash, true);
        }

        public bool IsCurrentAnimationFinished(float normalizedThreshold = 0.98f)
        {
            if (animator == null || animator.IsInTransition(0)) return false;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= normalizedThreshold;
        }

        private void OnValidate()
        {
            ApplyAnimatorSettings();
        }

        private IEnumerator HitReactionRoutine()
        {
            animator.SetBool(IsGettingHitHash, true);
            yield return new WaitForSeconds(hitReactionDuration);
            animator.SetBool(IsGettingHitHash, false);
            hitReactionRoutine = null;
        }

        private void ApplyAnimatorSettings()
        {
            if (animator == null) return;
            animator.applyRootMotion = useAnimatorRootMotion;
        }
    }
}
