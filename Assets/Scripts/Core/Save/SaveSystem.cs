using System.IO;
using TinyHunter.Core.Character;
using TinyHunter.Core.Equipment;
using TinyHunter.Core.Inventory;
using TinyHunter.Core.Quest;
using TinyHunter.Data.Items;
using TinyHunter.Data.Quests;
using UnityEngine;

namespace TinyHunter.Core.Save
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private QuestDefinition[] knownQuests;

        [Header("Future Character Creator Compatibility")]
        [SerializeField] private CharacterAppearanceData defaultAppearance = CharacterAppearanceData.Default();

        public static string SavePath => Path.Combine(Application.persistentDataPath, "tinyhunter_mvp_save.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool HasSave() => File.Exists(SavePath);

        public void SaveGame()
        {
            var inventory = FindFirstObjectByType<InventorySystem>();
            var equipment = FindFirstObjectByType<EquipmentSystem>();
            var questSystem = FindFirstObjectByType<QuestSystem>();
            var customizer = FindFirstObjectByType<CharacterCustomizer>();
            if (inventory == null || equipment == null || questSystem == null) return;

            SaveData data = new();
            foreach (var entry in inventory.Entries)
            {
                if (entry.Item == null) continue;
                data.inventory.Add(new SaveInventoryEntry { itemId = entry.Item.ItemId, quantity = entry.Quantity });
            }

            data.equippedWeaponId = equipment.EquippedWeapon != null ? equipment.EquippedWeapon.ItemId : null;
            data.equippedHeadArmorId = equipment.EquippedHead != null ? equipment.EquippedHead.ItemId : null;
            data.activeQuestId = questSystem.ActiveQuest != null ? questSystem.ActiveQuest.QuestId : null;

            if (data.profile == null) data.profile = new PlayerProfileData();
            if (data.profile.Appearance == null)
            {
                data.profile.Appearance = defaultAppearance != null ? defaultAppearance : CharacterAppearanceData.Default();
            }

            // Future hook: when creator UI is active, source appearance from profile manager/customizer.
            if (customizer == null)
            {
                // Keep current default-compatible profile data in save for forward compatibility.
                data.profile.Appearance = data.profile.Appearance ?? CharacterAppearanceData.Default();
            }

            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log($"Saved game: {SavePath}");
        }

        public void LoadGame()
        {
            if (!HasSave()) return;
            var inventory = FindFirstObjectByType<InventorySystem>();
            var equipment = FindFirstObjectByType<EquipmentSystem>();
            var questSystem = FindFirstObjectByType<QuestSystem>();
            var customizer = FindFirstObjectByType<CharacterCustomizer>();
            if (inventory == null || equipment == null || questSystem == null || itemDatabase == null) return;

            var json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            if (data == null) return;

            inventory.ClearAll();
            foreach (var saved in data.inventory)
            {
                var item = itemDatabase.FindById(saved.itemId);
                if (item != null) inventory.AddItem(item, saved.quantity);
            }

            if (!string.IsNullOrEmpty(data.equippedWeaponId))
            {
                var weapon = itemDatabase.FindById<WeaponDefinition>(data.equippedWeaponId);
                if (weapon != null) equipment.EquipWeapon(weapon);
            }

            if (!string.IsNullOrEmpty(data.equippedHeadArmorId))
            {
                var armor = itemDatabase.FindById<ArmorDefinition>(data.equippedHeadArmorId);
                if (armor != null) equipment.EquipArmor(armor);
            }

            if (!string.IsNullOrEmpty(data.activeQuestId))
            {
                foreach (var q in knownQuests)
                {
                    if (q != null && q.QuestId == data.activeQuestId)
                    {
                        questSystem.AcceptQuest(q);
                        break;
                    }
                }
            }

            if (data.profile == null) data.profile = new PlayerProfileData();
            if (data.profile.Appearance == null)
            {
                data.profile.Appearance = defaultAppearance != null ? defaultAppearance : CharacterAppearanceData.Default();
            }

            // Dormant apply hook: safe no-op unless CharacterCustomizer exists on active player prefab.
            if (customizer != null)
            {
                customizer.ApplyAppearance(data.profile.Appearance);
            }

            Debug.Log("Loaded game save.");
        }
    }
}
