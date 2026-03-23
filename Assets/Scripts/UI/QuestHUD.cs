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
        [Header("Layout")]
        [SerializeField] private bool enforceTopRightLayout = true;
        [SerializeField] private Vector2 topRightMargin = new(20f, 20f);
        [SerializeField] private float progressLineOffsetY = 24f;

        private void Awake()
        {
            if (questSystem == null) questSystem = FindFirstObjectByType<QuestSystem>();
            ApplyTopRightLayout();
            if (activeQuestText != null && string.IsNullOrWhiteSpace(activeQuestText.text)) activeQuestText.text = "Quest: None";
            if (progressText != null && string.IsNullOrWhiteSpace(progressText.text)) progressText.text = "Progress: 0/0";
        }

        private void ApplyTopRightLayout()
        {
            if (!enforceTopRightLayout) return;

            RectTransform root = transform as RectTransform;
            if (root != null)
            {
                root.anchorMin = new Vector2(1f, 1f);
                root.anchorMax = new Vector2(1f, 1f);
                root.pivot = new Vector2(1f, 1f);
                root.anchoredPosition = new Vector2(-topRightMargin.x, -topRightMargin.y);
            }

            PositionLine(activeQuestText, 0f);
            PositionLine(progressText, -progressLineOffsetY);
        }

        private static void PositionLine(Text text, float y)
        {
            if (text == null) return;

            RectTransform line = text.rectTransform;
            line.anchorMin = new Vector2(1f, 1f);
            line.anchorMax = new Vector2(1f, 1f);
            line.pivot = new Vector2(1f, 1f);
            line.anchoredPosition = new Vector2(0f, y);
        }

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
