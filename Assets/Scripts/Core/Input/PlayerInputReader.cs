using UnityEngine;

namespace TinyHunter.Core.Input
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour desktopSourceBehaviour;
        [SerializeField] private MonoBehaviour mobileSourceBehaviour;
        [SerializeField] private bool forceDesktopInput = true;

        private IGameInputSource activeSource;

        public Vector2 Move => activeSource != null ? activeSource.Move : Vector2.zero;
        public Vector2 Look => activeSource != null ? activeSource.Look : Vector2.zero;
        public bool JumpPressed => activeSource != null && activeSource.JumpPressed;
        public bool CrouchTogglePressed => activeSource != null && activeSource.CrouchTogglePressed;
        public bool DodgePressed => activeSource != null && activeSource.DodgePressed;
        public bool InteractPressed => activeSource != null && activeSource.InteractPressed;
        public bool PrimaryAttackPressed => activeSource != null && activeSource.PrimaryAttackPressed;
        public bool GuardHeld => activeSource != null && activeSource.GuardHeld;
        public bool LockTogglePressed => activeSource != null && activeSource.LockTogglePressed;
        public bool InventoryTogglePressed => activeSource != null && activeSource.InventoryTogglePressed;
        public bool EscapePressed => activeSource != null && activeSource.EscapePressed;
        public bool SavePressed => activeSource != null && activeSource.SavePressed;
        public bool LoadPressed => activeSource != null && activeSource.LoadPressed;

        private void Awake()
        {
            bool useMobile = !forceDesktopInput && Application.isMobilePlatform;
            var sourceBehaviour = useMobile ? mobileSourceBehaviour : desktopSourceBehaviour;
            activeSource = sourceBehaviour as IGameInputSource;

            if (activeSource == null)
            {
                Debug.LogWarning("PlayerInputReader missing valid input source. Falling back to desktop source if available.");
                activeSource = desktopSourceBehaviour as IGameInputSource;
            }
        }
    }
}
