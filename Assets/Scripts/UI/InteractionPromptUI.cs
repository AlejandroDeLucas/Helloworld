using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text promptText;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
            if (promptText != null && string.IsNullOrWhiteSpace(promptText.text)) promptText.text = "Press E";
        }

        public void Show(string text)
        {
            if (root != null) root.SetActive(true);
            if (promptText != null) promptText.text = text;
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }
}
