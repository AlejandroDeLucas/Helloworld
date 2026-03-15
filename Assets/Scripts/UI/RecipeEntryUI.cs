using TinyHunter.Core.Crafting;
using TinyHunter.Data.Crafting;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI
{
    public class RecipeEntryUI : MonoBehaviour
    {
        [SerializeField] private Text recipeName;
        [SerializeField] private Text requirements;
        [SerializeField] private Button craftButton;

        private CraftingRecipeDefinition recipe;
        private CraftingSystem craftingSystem;

        public void Bind(CraftingRecipeDefinition recipeDef, CraftingSystem system)
        {
            recipe = recipeDef;
            craftingSystem = system;

            if (recipeName != null) recipeName.text = recipe.DisplayName;
            if (requirements != null) requirements.text = BuildRequirementsText(recipe);

            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(Craft);
            Refresh();
        }

        public void Refresh()
        {
            if (craftButton != null && craftingSystem != null && recipe != null)
            {
                craftButton.interactable = craftingSystem.CanCraft(recipe);
            }
        }


        public bool TryCraftViaHotkey()
        {
            if (craftingSystem == null || recipe == null) return false;
            if (!craftingSystem.CanCraft(recipe)) return false;
            Craft();
            return true;
        }
        private void Craft()
        {
            if (craftingSystem.Craft(recipe))
            {
                Refresh();
            }
        }

        private static string BuildRequirementsText(CraftingRecipeDefinition recipeDef)
        {
            string text = string.Empty;
            foreach (var cost in recipeDef.Costs)
            {
                if (cost.Item == null) continue;
                text += $"{cost.Item.DisplayName} x{cost.Amount}\n";
            }

            return text.TrimEnd();
        }
    }
}
