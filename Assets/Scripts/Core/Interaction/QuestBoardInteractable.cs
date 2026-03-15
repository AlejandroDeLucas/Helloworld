using TinyHunter.Core.Quest;
using TinyHunter.Data.Quests;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Interaction
{
    public class QuestBoardInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private QuestSystem questSystem;
        [SerializeField] private QuestDefinition defaultQuest;
        [SerializeField] private DebugChecklistUI checklist;

        public string GetInteractionText() => "Accept Hunt Quest";

        public void Interact()
        {
            if (questSystem != null && defaultQuest != null)
            {
                questSystem.AcceptQuest(defaultQuest);
                checklist?.SetQuestAccepted();
            }
        }
    }
}
