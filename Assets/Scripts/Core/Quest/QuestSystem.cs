using System;
using TinyHunter.Core.Inventory;
using TinyHunter.Data.Quests;
using UnityEngine;

namespace TinyHunter.Core.Quest
{
    public class QuestSystem : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventory;

        public QuestDefinition ActiveQuest { get; private set; }
        public int CurrentProgress { get; private set; }
        public bool IsQuestComplete { get; private set; }

        public event Action<QuestDefinition> OnQuestAccepted;
        public event Action<QuestDefinition, int, int> OnQuestProgressChanged;
        public event Action<QuestDefinition> OnQuestCompleted;

        public void AcceptQuest(QuestDefinition quest)
        {
            ActiveQuest = quest;
            CurrentProgress = 0;
            IsQuestComplete = false;
            OnQuestAccepted?.Invoke(ActiveQuest);
            OnQuestProgressChanged?.Invoke(ActiveQuest, CurrentProgress, ActiveQuest.TargetCount);
        }

        public void RegisterTargetDefeated(string monsterId)
        {
            if (ActiveQuest == null || ActiveQuest.TargetMonster == null || IsQuestComplete) return;
            if (ActiveQuest.TargetMonster.MonsterId != monsterId) return;

            CurrentProgress++;
            OnQuestProgressChanged?.Invoke(ActiveQuest, CurrentProgress, ActiveQuest.TargetCount);

            if (CurrentProgress < ActiveQuest.TargetCount) return;

            IsQuestComplete = true;
            if (ActiveQuest.RewardItem != null && ActiveQuest.RewardAmount > 0 && inventory != null)
            {
                inventory.AddItem(ActiveQuest.RewardItem, ActiveQuest.RewardAmount);
            }

            OnQuestCompleted?.Invoke(ActiveQuest);
            Debug.Log($"Quest complete: {ActiveQuest.DisplayName}");
        }
    }
}
