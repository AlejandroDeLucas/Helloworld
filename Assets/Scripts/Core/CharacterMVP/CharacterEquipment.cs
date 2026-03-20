using System.Collections.Generic;
using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class CharacterEquipment : MonoBehaviour
    {
        [SerializeField] private CharacterSocketMap socketMap;

        private readonly Dictionary<EquipmentSlotType, GameObject> equippedObjects = new();

        private void Reset()
        {
            if (socketMap == null) socketMap = GetComponent<CharacterSocketMap>();
        }

        public GameObject Equip(EquipmentSlotType slot, GameObject prefab)
        {
            if (socketMap == null || prefab == null) return null;

            Transform socket = socketMap.GetSocket(slot);
            if (socket == null) return null;

            Unequip(slot);
            GameObject instance = Instantiate(prefab, socket);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            equippedObjects[slot] = instance;
            return instance;
        }

        public void Unequip(EquipmentSlotType slot)
        {
            if (!equippedObjects.TryGetValue(slot, out var current) || current == null) return;
            Destroy(current);
            equippedObjects.Remove(slot);
        }

        public GameObject GetEquippedObject(EquipmentSlotType slot)
        {
            equippedObjects.TryGetValue(slot, out var current);
            return current;
        }
    }
}
