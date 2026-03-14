using TinyHunter.Core.Input;
using TinyHunter.Core.Inventory;
using TinyHunter.Core.Player;
using TinyHunter.UI;
using UnityEngine;

namespace TinyHunter.Core.Interaction
{
    public class PlayerInteractionSystem : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private float interactRange = 1.6f;
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private InteractionPromptUI promptUI;

        private IInteractable currentInteractable;
        private WorldPickup currentPickup;

        private void Update()
        {
            if (input == null || playerController == null) return;
            if (!playerController.CanAct())
            {
                if (promptUI != null) promptUI.Hide();
                return;
            }

            Scan();

            if (input.InteractPressed)
            {
                if (currentPickup != null)
                {
                    currentPickup.TryCollect(inventory);
                    return;
                }

                currentInteractable?.Interact();
            }
        }

        private void Scan()
        {
            currentInteractable = null;
            currentPickup = null;
            if (promptUI != null) promptUI.Hide();

            var hits = Physics.OverlapSphere(transform.position, interactRange, interactionMask);
            float best = float.MaxValue;

            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance > best) continue;

                if (hit.TryGetComponent<WorldPickup>(out var pickup))
                {
                    best = distance;
                    currentPickup = pickup;
                    currentInteractable = null;
                    continue;
                }

                var interactable = hit.GetComponent(typeof(IInteractable)) as IInteractable;
                if (interactable != null)
                {
                    best = distance;
                    currentInteractable = interactable;
                    currentPickup = null;
                }
            }

            if (currentPickup != null)
            {
                if (promptUI != null) promptUI.Show($"[E] {currentPickup.DisplayLabel}");
            }
            else if (currentInteractable != null)
            {
                if (promptUI != null) promptUI.Show($"[E] {currentInteractable.GetInteractionText()}");
            }
        }
    }
}
