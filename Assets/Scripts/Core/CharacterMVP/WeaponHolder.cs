using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class WeaponHolder : MonoBehaviour
    {
        [SerializeField] private CharacterEquipment characterEquipment;
        [SerializeField] private EquipmentSlotType weaponSlot = EquipmentSlotType.RightHand;

        public GameObject EquippedWeapon { get; private set; }

        private void Reset()
        {
            if (characterEquipment == null) characterEquipment = GetComponent<CharacterEquipment>();
        }

        public GameObject EquipWeapon(GameObject weaponPrefab)
        {
            if (characterEquipment == null) return null;
            EquippedWeapon = characterEquipment.Equip(weaponSlot, weaponPrefab);
            return EquippedWeapon;
        }

        public void UnequipWeapon()
        {
            if (characterEquipment == null) return;
            characterEquipment.Unequip(weaponSlot);
            EquippedWeapon = null;
        }

        public void SetWeaponSlot(EquipmentSlotType slot)
        {
            weaponSlot = slot;
        }
    }
}
