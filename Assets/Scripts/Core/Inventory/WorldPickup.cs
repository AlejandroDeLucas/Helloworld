using TinyHunter.Core.Player;
using TinyHunter.Data.Items;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Inventory
{
    public class WorldPickup : MonoBehaviour
    {
        [SerializeField] private ItemDefinition item;
        [SerializeField] private int amount = 1;
        [SerializeField] private bool autoPickupOnTrigger;
        [SerializeField] private DebugChecklistUI checklist;

        private InventorySystem playerInventory;

        public string DisplayLabel => item == null ? "Pickup" : $"Pick up {item.DisplayName} x{amount}";

        public void Setup(ItemDefinition definition, int quantity)
        {
            item = definition;
            amount = quantity;
        }

        public bool TryCollect(InventorySystem inventory)
        {
            if (item == null || inventory == null) return false;
            inventory.AddItem(item, amount);
            checklist?.SetPickedLoot();
            Destroy(gameObject);
            return true;
        }

        private void Start()
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerInventory = player.GetComponent<InventorySystem>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!autoPickupOnTrigger) return;
            if (!other.GetComponent<PlayerController>()) return;
            TryCollect(playerInventory);
        }
    }
}
