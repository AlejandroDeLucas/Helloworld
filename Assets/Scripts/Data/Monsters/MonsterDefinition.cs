using TinyHunter.Data.Combat;
using UnityEngine;

namespace TinyHunter.Data.Monsters
{
    public enum MonsterSizeCategory { Small, Medium, Large, Flagship }
    public enum AggressionLevel { Passive, Defensive, Aggressive, Frenzied }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Monster")]
    public class MonsterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string MonsterId;
        public string DisplayName;
        [TextArea] public string EcologicalRole;
        [TextArea] public string Territory;

        [Header("Combat")]
        public MonsterSizeCategory SizeCategory;
        public AggressionLevel Aggression;
        public float MaxHealth = 400f;
        public float BaseAttack = 15f;
        public float MoveSpeed = 2f;
        public DamageType Weakness;

        [Header("Anatomy")]
        public MonsterPartDefinition[] BreakableParts;

        [Header("Rewards")]
        public LootTableDefinition LootTable;
    }
}
