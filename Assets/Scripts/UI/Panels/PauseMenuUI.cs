using UnityEngine;

namespace TinyHunter.UI.Panels
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private GameObject deathLabel;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        private void Start()
        {
            Close();
            if (deathLabel != null) deathLabel.SetActive(false);
        }

        public void TogglePause()
        {
            if (panelRoot == null) return;
            bool next = !panelRoot.activeSelf;
            panelRoot.SetActive(next);
            Time.timeScale = next ? 0f : 1f;
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            Time.timeScale = 1f;
        }

        public void ShowDeathState()
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            if (deathLabel != null) deathLabel.SetActive(true);
        }
    }
}
