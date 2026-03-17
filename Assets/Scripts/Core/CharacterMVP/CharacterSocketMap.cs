using System;
using System.Collections.Generic;
using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public enum EquipmentSlotType
    {
        RightHand = 0,
        LeftHand = 1,
        Back = 2,
        Head = 3,
        Shoulders = 4,
        Belt = 5,
        Beard = 6,
        Backpack = 7,
        Chest = 8,
        Legs = 9
    }

    [Serializable]
    public struct SocketBinding
    {
        public EquipmentSlotType Slot;
        public Transform Socket;
    }

    public class CharacterSocketMap : MonoBehaviour
    {
        [SerializeField] private List<SocketBinding> sockets = new();

        public Transform GetSocket(EquipmentSlotType slot)
        {
            foreach (var binding in sockets)
            {
                if (binding.Slot == slot)
                {
                    return binding.Socket;
                }
            }

            return null;
        }
    }
}
