using UnityEngine;
using UnityEngine.Serialization;

namespace TinyHunter.Data.Items
{
    public enum ArmorSlot { Head, Chest, Arms, Waist, Legs }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Armor")]
    // Hotfix note: ArmorDefinition must inherit ItemDefinition for crafting/equipment compatibility.
    public class ArmorDefinition : ItemDefinition
    {
        [FormerlySerializedAs("ArmorId")]
        [Header("Armor")]
        public string ArmorLegacyId;

        public ArmorSlot Slot;
        public int Defense;
        public float PoisonResistance;
        public float AcidResistance;
        public float WaterResistance;
        public float ImpactResistance;
        public float StaminaModifier;
        public float DodgeEfficiencyModifier;

        private void OnValidate()
        {
            Category = ItemCategory.Armor;
            if (!string.IsNullOrEmpty(ArmorLegacyId) && string.IsNullOrEmpty(ItemId))
            {
                ItemId = ArmorLegacyId;
            }
        }
    }
}
