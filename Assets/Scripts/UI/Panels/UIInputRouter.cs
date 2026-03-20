using TinyHunter.Core.Input;
using TinyHunter.Core.Player;
using TinyHunter.Core.Save;
using UnityEngine;

namespace TinyHunter.UI.Panels
{
    public class UIInputRouter : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private InventoryPanelUI inventoryPanel;
        [SerializeField] private EquipmentPanelUI equipmentPanel;
        [SerializeField] private PauseMenuUI pauseMenu;
        [SerializeField] private TinyHunter.UI.CraftingPanelUI craftingPanel;
        [SerializeField] private SaveSystem saveSystem;
        [SerializeField] private DebugChecklistUI checklist;

        private void Update()
        {
            if (input == null) return;

            if (input.InventoryTogglePressed)
            {
                inventoryPanel?.Toggle();
                equipmentPanel?.Toggle();
                SyncPlayerInteraction();
            }

            if (input.EscapePressed)
            {
                if (craftingPanel != null && craftingPanel.IsOpen)
                {
                    craftingPanel.Close();
                }
                else if (inventoryPanel != null && inventoryPanel.IsOpen)
                {
                    inventoryPanel.Close();
                    equipmentPanel?.Close();
                }
                else
                {
                    pauseMenu?.TogglePause();
                }

                SyncPlayerInteraction();
            }

            if (input.SavePressed)
            {
                saveSystem?.SaveGame();
                checklist?.SetSavedGame();
            }

            if (input.LoadPressed)
            {
                saveSystem?.LoadGame();
            }
        }

        private void SyncPlayerInteraction()
        {
            bool anyPanel = (craftingPanel != null && craftingPanel.IsOpen)
                            || (inventoryPanel != null && inventoryPanel.IsOpen)
                            || (equipmentPanel != null && equipmentPanel.IsOpen)
                            || (pauseMenu != null && pauseMenu.IsOpen);
            playerController?.SetInteractionState(anyPanel);
        }
    }
}
