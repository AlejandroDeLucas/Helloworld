using System;
using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Data.Monsters
{
    public enum LootRarity
    {
        Common,
        Uncommon,
        Rare,
        VeryRare
    }

    [Serializable]
    public struct LootEntry
    {
        public ItemDefinition Item;
        public LootRarity Rarity;
        [Range(0f, 1f)] public float DropChance;
        public int MinAmount;
        public int MaxAmount;
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Loot Table")]
    public class LootTableDefinition : ScriptableObject
    {
        public LootEntry[] Entries;
    }
}
