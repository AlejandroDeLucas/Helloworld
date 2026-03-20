using TinyHunter.Core.Equipment;
using TinyHunter.Data.Items;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI.Panels
{
    public class EquipmentPanelUI : MonoBehaviour
    {
        [SerializeField] private EquipmentSystem equipment;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text weaponText;
        [SerializeField] private Text headText;
        [SerializeField] private InventoryPanelUI inventoryPanel;

        private void Start()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            if (equipment != null) equipment.OnEquipmentChanged += Refresh;
            Refresh();
        }

        public bool IsOpen => panelRoot != null && panelRoot.activeSelf;

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
            if (weaponText != null)
            {
                weaponText.text = equipment != null && equipment.EquippedWeapon != null
                    ? $"Weapon: {equipment.EquippedWeapon.DisplayName}"
                    : "Weapon: None";
            }

            if (headText != null)
            {
                headText.text = equipment != null && equipment.EquippedHead != null
                    ? $"Head: {equipment.EquippedHead.DisplayName}"
                    : "Head: None";
            }
        }

        public void EquipWeapon(WeaponDefinition weapon) => equipment?.EquipWeapon(weapon);
        public void EquipHead(ArmorDefinition armor) => equipment?.EquipArmor(armor);
    }
}
