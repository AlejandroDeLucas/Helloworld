using UnityEngine;

namespace TinyHunter.Core.Input
{
    public class MobileInputSourceStub : MonoBehaviour, IGameInputSource
    {
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public bool JumpPressed { get; private set; }
        public bool CrouchTogglePressed { get; private set; }
        public bool DodgePressed { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool InteractPressed { get; private set; }
        public bool PrimaryAttackPressed { get; private set; }
        public bool GuardHeld { get; private set; }
        public bool LockTogglePressed { get; private set; }
        public bool InventoryTogglePressed { get; private set; }
        public bool EscapePressed { get; private set; }
        public bool SavePressed => false;
        public bool LoadPressed => false;

        // Future UI button hooks
        public void SetMove(Vector2 value) => Move = value;
        public void SetLook(Vector2 value) => Look = value;
        public void PressJump() => JumpPressed = true;
        public void ToggleCrouch() => CrouchTogglePressed = true;
        public void PressDodge() => DodgePressed = true;
        public void SetSprint(bool value) => SprintHeld = value;
        public void PressInteract() => InteractPressed = true;
        public void PressPrimaryAttack() => PrimaryAttackPressed = true;
        public void SetGuard(bool value) => GuardHeld = value;
        public void ToggleLock() => LockTogglePressed = true;
        public void ToggleInventory() => InventoryTogglePressed = true;
        public void PressEscape() => EscapePressed = true;

        private void LateUpdate()
        {
            JumpPressed = false;
            CrouchTogglePressed = false;
            DodgePressed = false;
            InteractPressed = false;
            PrimaryAttackPressed = false;
            LockTogglePressed = false;
            InventoryTogglePressed = false;
            EscapePressed = false;
            Look = Vector2.zero;
        }
    }
}
