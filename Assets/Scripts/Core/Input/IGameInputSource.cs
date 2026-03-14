using UnityEngine;

namespace TinyHunter.Core.Input
{
    public interface IGameInputSource
    {
        Vector2 Move { get; }
        Vector2 Look { get; }

        bool JumpPressed { get; }
        bool CrouchTogglePressed { get; }
        bool DodgePressed { get; }
        bool InteractPressed { get; }
        bool PrimaryAttackPressed { get; }
        bool GuardHeld { get; }
        bool LockTogglePressed { get; }
        bool InventoryTogglePressed { get; }
        bool EscapePressed { get; }
        bool SavePressed { get; }
        bool LoadPressed { get; }
    }
}
