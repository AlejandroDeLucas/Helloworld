using UnityEngine;

namespace TinyHunter.Core.Input
{
    public class DesktopInputSource : MonoBehaviour, IGameInputSource
    {
        public Vector2 Move => new(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
        public Vector2 Look => new(UnityEngine.Input.GetAxisRaw("Mouse X"), UnityEngine.Input.GetAxisRaw("Mouse Y"));

        public bool JumpPressed => UnityEngine.Input.GetKeyDown(KeyCode.Space);
        public bool CrouchTogglePressed => UnityEngine.Input.GetKeyDown(KeyCode.C);
        public bool DodgePressed => UnityEngine.Input.GetKeyDown(KeyCode.LeftShift);
        public bool SprintHeld => UnityEngine.Input.GetKey(KeyCode.LeftShift);
        public bool InteractPressed => UnityEngine.Input.GetKeyDown(KeyCode.E);
        public bool PrimaryAttackPressed => UnityEngine.Input.GetMouseButtonDown(0);
        public bool GuardHeld => UnityEngine.Input.GetMouseButton(1);
        public bool LockTogglePressed => UnityEngine.Input.GetKeyDown(KeyCode.R);
        public bool InventoryTogglePressed => UnityEngine.Input.GetKeyDown(KeyCode.Tab);
        public bool EscapePressed => UnityEngine.Input.GetKeyDown(KeyCode.Escape);
        public bool SavePressed => UnityEngine.Input.GetKeyDown(KeyCode.F5);
        public bool LoadPressed => UnityEngine.Input.GetKeyDown(KeyCode.F9);
    }
}
