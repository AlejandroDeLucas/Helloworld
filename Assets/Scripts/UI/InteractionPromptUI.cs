using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI
{
    public class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text promptText;

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
