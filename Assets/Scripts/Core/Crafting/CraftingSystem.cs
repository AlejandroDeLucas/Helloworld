using System;
using TinyHunter.Core.Inventory;
using TinyHunter.Data.Crafting;
using UnityEngine;

namespace TinyHunter.Core.Crafting
{
    public class CraftingSystem : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventory;

        public event Action<CraftingRecipeDefinition> OnCrafted;

        public bool CanCraft(CraftingRecipeDefinition recipe)
        {
            foreach (var cost in recipe.Costs)
            {
                if (!inventory.HasItem(cost.Item, cost.Amount))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Craft(CraftingRecipeDefinition recipe)
        {
            if (!CanCraft(recipe)) return false;

            foreach (var cost in recipe.Costs)
            {
                inventory.RemoveItem(cost.Item, cost.Amount);
            }

            inventory.AddItem(recipe.OutputItem, recipe.OutputAmount);
            OnCrafted?.Invoke(recipe);
            return true;
        }
    }
}
