using System;
using System.Collections.Generic;
using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Core.Inventory
{
    [Serializable]
    public struct InventoryEntry
    {
        public ItemDefinition Item;
        public int Quantity;
    }

    public class InventorySystem : MonoBehaviour
    {
        [SerializeField] private List<InventoryEntry> entries = new();
        public event Action<ItemDefinition, int> OnItemChanged;

        public IReadOnlyList<InventoryEntry> Entries => entries;

        public bool HasItem(ItemDefinition item, int amount)
        {
            foreach (var entry in entries)
            {
                if (entry.Item == item && entry.Quantity >= amount)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddItem(ItemDefinition item, int amount)
        {
            if (item == null || amount <= 0) return;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Item == item)
                {
                    var entry = entries[i];
                    entry.Quantity += amount;
                    entries[i] = entry;
                    OnItemChanged?.Invoke(item, entry.Quantity);
                    return;
                }
            }

            entries.Add(new InventoryEntry { Item = item, Quantity = amount });
            OnItemChanged?.Invoke(item, amount);
        }

        public bool RemoveItem(ItemDefinition item, int amount)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Item != item) continue;
                if (entries[i].Quantity < amount) return false;

                var entry = entries[i];
                entry.Quantity -= amount;

                if (entry.Quantity <= 0)
                {
                    entries.RemoveAt(i);
                    OnItemChanged?.Invoke(item, 0);
                    return true;
                }

                entries[i] = entry;
                OnItemChanged?.Invoke(item, entry.Quantity);
                return true;
            }

            return false;
        }

        public void ClearAll()
        {
            entries.Clear();
        }
    }
}
