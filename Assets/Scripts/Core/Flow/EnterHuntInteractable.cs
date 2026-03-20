using TinyHunter.Core.Interaction;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Flow
{
    public class EnterHuntInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private DebugChecklistUI checklist;

        public string GetInteractionText() => "Depart to Kitchen Hunt";

        public void Interact()
        {
            checklist?.SetEnteredHunt();
            SceneFlowController.Instance.EnterHunt();
        }
    }
}
