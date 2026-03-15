using TinyHunter.Data.Items;
using TinyHunter.Data.Monsters;
using UnityEngine;

namespace TinyHunter.Data.Quests
{
    public enum QuestType
    {
        SlayTarget,
        CaptureTarget,
        GatherMaterials,
        EliminateInfestation,
        InvestigateNest
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Quest")]
    public class QuestDefinition : ScriptableObject
    {
        public string QuestId;
        public string DisplayName;
        [TextArea] public string Objective;
        public QuestType QuestType;
        public MonsterDefinition TargetMonster;
        public int TargetCount = 1;
        public float TimeLimitSeconds = 1200f;
        public ItemDefinition RewardItem;
        public int RewardAmount;
    }
}
