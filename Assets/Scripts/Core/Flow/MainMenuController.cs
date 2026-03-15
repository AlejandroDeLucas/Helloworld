using TinyHunter.Core.Save;
using UnityEngine;

namespace TinyHunter.Core.Flow
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private SaveSystem saveSystem;

        public void StartGame() => SceneFlowController.Instance.StartNewGame();

        public void ContinueGame() => SceneFlowController.Instance.ContinueGame();

        public bool CanContinue() => saveSystem != null && saveSystem.HasSave();

        public void QuitGame() => Application.Quit();
    }
}
