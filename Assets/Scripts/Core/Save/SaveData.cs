using System;
using System.Collections.Generic;
using TinyHunter.Core.Character;

namespace TinyHunter.Core.Save
{
    [Serializable]
    public class SaveInventoryEntry
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public class SaveData
    {
        public List<SaveInventoryEntry> inventory = new();
        public string equippedWeaponId;
        public string equippedHeadArmorId;
        public string activeQuestId;

        // Future-compatible creator/profile data (dormant until creator flow is activated).
        public PlayerProfileData profile = new PlayerProfileData();
    }
}
