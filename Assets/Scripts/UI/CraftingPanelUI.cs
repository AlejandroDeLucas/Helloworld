using System.Collections.Generic;
using TinyHunter.Core.Crafting;
using TinyHunter.Core.Equipment;
using TinyHunter.Core.Input;
using TinyHunter.Data.Crafting;
using TinyHunter.Data.Items;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.UI
{
    // Hotfix note: keep output item handling explicit for WeaponDefinition/ArmorDefinition craft equips.
    public class CraftingPanelUI : MonoBehaviour
    {
        [SerializeField] private CraftingSystem craftingSystem;
        [SerializeField] private EquipmentSystem equipmentSystem;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private DebugChecklistUI checklist;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private RecipeEntryUI recipeEntryPrefab;
        [SerializeField] private Transform recipeContainer;
        [SerializeField] private CraftingRecipeDefinition[] recipes;

        private readonly List<RecipeEntryUI> entries = new();

        private void Start()
        {
            BuildEntries();
            if (panelRoot != null) panelRoot.SetActive(false);
            if (craftingSystem != null) craftingSystem.OnCrafted += HandleCrafted;
        }

        private void Update()
        {
            if (panelRoot == null || !panelRoot.activeSelf || input == null) return;
            if (input.InteractPressed)
            {
                foreach (var entry in entries)
                {
                    if (entry.TryCraftViaHotkey())
                    {
                        break;
                    }
                }
            }
        }

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Toggle()
        {
            if (panelRoot == null) return;
            panelRoot.SetActive(!panelRoot.activeSelf);
            Refresh();
        }

        public void Refresh()
        {
            foreach (var entry in entries)
            {
                entry.Refresh();
            }
        }

        private void BuildEntries()
        {
            foreach (var recipe in recipes)
            {
                if (recipe == null) continue;
                var entry = Instantiate(recipeEntryPrefab, recipeContainer);
                entry.Bind(recipe, craftingSystem);
                entries.Add(entry);
            }
        }

        private void HandleCrafted(CraftingRecipeDefinition recipe)
        {
            // Hotfix note: the crafted output can auto-equip when the recipe produces gear.
            checklist?.SetCraftedItem();
            if (recipe.OutputItem is WeaponDefinition weapon)
            {
                equipmentSystem?.EquipWeapon(weapon);
                checklist?.SetEquippedItem();
            }
            else if (recipe.OutputItem is ArmorDefinition armor)
            {
                equipmentSystem?.EquipArmor(armor);
                checklist?.SetEquippedItem();
            }

            Refresh();
        }
    }
}
