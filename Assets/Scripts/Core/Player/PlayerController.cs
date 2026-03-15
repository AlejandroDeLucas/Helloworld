using TinyHunter.Core.Combat;
using TinyHunter.Core.Input;
using UnityEngine;

namespace TinyHunter.Core.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public enum PlayerState { Movement, Attacking, Dodging, Interacting, Dead }

        [SerializeField] private PlayerStats stats;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private TargetLockSystem lockSystem;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float jumpHeight = 0.8f;
        [SerializeField] private float dodgeDistance = 1.2f;
        [SerializeField] private float dodgeStaminaCost = 20f;
        [SerializeField] private float dodgeDuration = 0.22f;
        [SerializeField] private float crouchSpeedMultiplier = 0.65f;
        [SerializeField] private PlayerAnimationBridge animationBridge;

        public bool IsCrouching { get; private set; }
        public PlayerState CurrentState { get; private set; } = PlayerState.Movement;

        private CharacterController controller;
        private Vector3 velocity;
        private float stateTimer;

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            if (stats == null || input == null) return;

            if (stats.IsDead)
            {
                CurrentState = PlayerState.Dead;
                return;
            }

            if (input.LockTogglePressed) lockSystem?.ToggleLock();
            if (input.CrouchTogglePressed) ToggleCrouch();

            UpdateStateMachine();
            stats.RegenerateStamina(16f);
        }

        private void UpdateStateMachine()
        {
            if (CurrentState == PlayerState.Dodging)
            {
                animationBridge?.SetMoveSpeed(0f);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) CurrentState = PlayerState.Movement;
                ApplyGravity();
                return;
            }

            if (CurrentState == PlayerState.Interacting || CurrentState == PlayerState.Attacking)
            {
                animationBridge?.SetMoveSpeed(0f);
                return;
            }

            if (input.DodgePressed && stats.SpendStamina(dodgeStaminaCost))
            {
                StartDodge();
                return;
            }

            Move();
            HandleJump();
            ApplyGravity();
        }

        private void Move()
        {
            Vector2 moveInput = input.Move;
            Vector3 inputDir = new(moveInput.x, 0f, moveInput.y);

            Vector3 forward = cameraPivot.forward;
            Vector3 right = cameraPivot.right;
            forward.y = 0f;
            right.y = 0f;
            Vector3 move = (forward.normalized * inputDir.z + right.normalized * inputDir.x).normalized;

            if (lockSystem != null && lockSystem.CurrentTarget != null)
            {
                Vector3 toTarget = lockSystem.CurrentTarget.position - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget), Time.deltaTime * 10f);
                }
            }
            else if (move.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 10f);
            }

            float speed = stats.MoveSpeed * (IsCrouching ? crouchSpeedMultiplier : 1f);
            float normalizedMoveSpeed = moveInput.magnitude;
            animationBridge?.SetMoveSpeed(normalizedMoveSpeed);
            controller.Move(move * speed * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (input.JumpPressed && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && velocity.y < 0f)
            {
                velocity.y = -1f;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }

        private void StartDodge()
        {
            CurrentState = PlayerState.Dodging;
            stateTimer = dodgeDuration;
            animationBridge?.TriggerDodge();
            Vector3 dodgeDir = transform.forward * dodgeDistance * stats.DodgeEfficiency;
            controller.Move(dodgeDir);
        }

        private void ToggleCrouch()
        {
            IsCrouching = !IsCrouching;
            controller.height = IsCrouching ? 1f : 1.8f;
            controller.center = IsCrouching ? new Vector3(0f, 0.5f, 0f) : new Vector3(0f, 0.9f, 0f);
        }

        public bool CanAct() => CurrentState == PlayerState.Movement;

        public void SetInteractionState(bool interacting)
        {
            if (stats != null && stats.IsDead) return;
            CurrentState = interacting ? PlayerState.Interacting : PlayerState.Movement;
        }

        public void SetAttackState(float duration)
        {
            if (!CanAct()) return;
            CurrentState = PlayerState.Attacking;
            stateTimer = duration;
            StartCoroutine(EndAttackState());
        }

        private System.Collections.IEnumerator EndAttackState()
        {
            yield return new WaitForSeconds(stateTimer);
            if (CurrentState == PlayerState.Attacking) CurrentState = PlayerState.Movement;
        }
    }
}
