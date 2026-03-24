using System;
using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Data.Crafting
{
    [Serializable]
    public struct MaterialCost
    {
        public ItemDefinition Item;
        public int Amount;
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Crafting Recipe")]
    public class CraftingRecipeDefinition : ScriptableObject
    {
        public string RecipeId;
        public string DisplayName;
        public ItemCategory OutputCategory;
        public ItemDefinition OutputItem;
        public int OutputAmount = 1;
        public string UnlockCondition;
        public MaterialCost[] Costs;
    }
}
