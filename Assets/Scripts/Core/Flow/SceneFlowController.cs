using TinyHunter.Core.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TinyHunter.Core.Flow
{
    public class SceneFlowController : MonoBehaviour
    {
        public static SceneFlowController Instance { get; private set; }

        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string hubScene = "Hub_MVP";
        [SerializeField] private string huntScene = "Kitchen_MVP";

        private bool loadOnHubEnter;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == hubScene && loadOnHubEnter)
            {
                SaveSystem.Instance?.LoadGame();
                loadOnHubEnter = false;
            }
        }

        public void GoToMainMenu() => SceneManager.LoadScene(mainMenuScene);
        public void StartNewGame() => SceneManager.LoadScene(hubScene);

        public void ContinueGame()
        {
            loadOnHubEnter = true;
            SceneManager.LoadScene(hubScene);
        }

        public void EnterHunt() => SceneManager.LoadScene(huntScene);
        public void ReturnToHub() => SceneManager.LoadScene(hubScene);
    }
}
