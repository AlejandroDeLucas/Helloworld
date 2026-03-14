using UnityEngine;

namespace TinyHunter.Data.Items
{
    public enum ArmorSlot { Head, Chest, Arms, Waist, Legs }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Armor")]
    public class ArmorDefinition : ScriptableObject
    {
        public string ArmorId;
        public string DisplayName;
        public ArmorSlot Slot;
        public int Defense;
        public float PoisonResistance;
        public float AcidResistance;
        public float WaterResistance;
        public float ImpactResistance;
        public float StaminaModifier;
        public float DodgeEfficiencyModifier;
    }
}
