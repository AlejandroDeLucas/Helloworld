using TinyHunter.Core.Quest;
using TinyHunter.Data.Quests;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI
{
    public class QuestHUD : MonoBehaviour
    {
        [SerializeField] private QuestSystem questSystem;
        [SerializeField] private Text activeQuestText;
        [SerializeField] private Text progressText;

        private void OnEnable()
        {
            if (questSystem == null) return;
            questSystem.OnQuestAccepted += HandleQuestAccepted;
            questSystem.OnQuestProgressChanged += HandleProgress;
            questSystem.OnQuestCompleted += HandleCompleted;
        }

        private void OnDisable()
        {
            if (questSystem == null) return;
            questSystem.OnQuestAccepted -= HandleQuestAccepted;
            questSystem.OnQuestProgressChanged -= HandleProgress;
            questSystem.OnQuestCompleted -= HandleCompleted;
        }

        private void HandleQuestAccepted(QuestDefinition quest)
        {
            if (activeQuestText != null) activeQuestText.text = $"Quest: {quest.DisplayName}";
            if (progressText != null) progressText.text = $"Progress: 0/{quest.TargetCount}";
        }

        private void HandleProgress(QuestDefinition quest, int current, int target)
        {
            if (progressText != null) progressText.text = $"Progress: {current}/{target}";
        }

        private void HandleCompleted(QuestDefinition quest)
        {
            if (progressText != null) progressText.text = $"Completed: {quest.DisplayName}";
        }
    }
}
