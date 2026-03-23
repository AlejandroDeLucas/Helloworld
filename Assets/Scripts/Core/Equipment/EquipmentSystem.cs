using System;
using TinyHunter.Data.Items;
using UnityEngine;

namespace TinyHunter.Core.Equipment
{
    public class EquipmentSystem : MonoBehaviour
    {
        public WeaponDefinition EquippedWeapon { get; private set; }
        public ArmorDefinition EquippedHead { get; private set; }

        public event Action OnEquipmentChanged;

        public void EquipWeapon(WeaponDefinition weapon)
        {
            EquippedWeapon = weapon;
            OnEquipmentChanged?.Invoke();
        }

        public void EquipArmor(ArmorDefinition armor)
        {
            if (armor == null) return;
            if (armor.Slot == ArmorSlot.Head)
            {
                EquippedHead = armor;
            }

            OnEquipmentChanged?.Invoke();
        }
    }
}
