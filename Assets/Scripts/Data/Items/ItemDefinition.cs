using UnityEngine;

namespace TinyHunter.Data.Items
{
    public enum ItemCategory
    {
        Material,
        Consumable,
        Weapon,
        Armor,
        Tool,
        Quest
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Item")]
    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string ItemId;
        public string DisplayName;
        [TextArea] public string Description;
        public ItemCategory Category;

        [Header("Economy")]
        public int SellValue;
        public bool Stackable = true;
        public int MaxStack = 99;

        [Header("Presentation")]
        public Sprite Icon;
        public GameObject WorldPickupPrefab;
    }
}
