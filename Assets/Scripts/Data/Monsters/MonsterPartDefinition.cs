using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Data.Monsters
{
    [CreateAssetMenu(menuName = "TinyHunter/Data/Monster Part")]
    public class MonsterPartDefinition : ScriptableObject
    {
        public string PartId;
        public string DisplayName;
        public float MaxDurability = 100f;
        public float BreakDamageMultiplier = 1f;

        [Header("Combat Impact")]
        public string GameplayEffectOnBreak;

        [Header("Rewards")]
        public ItemDefinition GuaranteedDrop;
        public int GuaranteedDropAmount = 1;
    }
}
