using System.Text;
using TinyHunter.Core.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI.Panels
{
    public class InventoryPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private InventorySystem inventory;
        [SerializeField] private Text contentText;

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

        private void Start()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            Refresh();
            if (inventory != null) inventory.OnItemChanged += (_, _) => Refresh();
        }

        public void Toggle()
        {
            if (panelRoot == null) return;
            panelRoot.SetActive(!panelRoot.activeSelf);
            Refresh();
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        public void Refresh()
        {
            if (contentText == null || inventory == null) return;

            StringBuilder sb = new();
            foreach (var entry in inventory.Entries)
            {
                if (entry.Item == null) continue;
                sb.AppendLine($"- {entry.Item.DisplayName} x{entry.Quantity}");
            }

            contentText.text = sb.Length == 0 ? "Inventory Empty" : sb.ToString();
        }
    }
}
