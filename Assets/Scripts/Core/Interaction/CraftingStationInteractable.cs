using TinyHunter.Core.Player;
using TinyHunter.UI;
using UnityEngine;

namespace TinyHunter.Core.Interaction
{
    public class CraftingStationInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private CraftingPanelUI craftingPanel;
        [SerializeField] private PlayerController playerController;

        public string GetInteractionText() => "Open Crafting";

        public void Interact()
        {
            if (craftingPanel != null)
            {
                craftingPanel.Toggle();
                playerController?.SetInteractionState(craftingPanel.IsOpen);
            }
        }
    }
}
