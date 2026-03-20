using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Flow
{
    public class HuntExitTrigger : MonoBehaviour
    {
        [SerializeField] private DebugChecklistUI checklist;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<TinyHunter.Core.Player.PlayerController>() == null) return;
            checklist?.SetReturnedHub();
            SceneFlowController.Instance.ReturnToHub();
        }
    }
}
