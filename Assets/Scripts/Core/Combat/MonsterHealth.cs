using TinyHunter.Core.Inventory;
using TinyHunter.Core.Quest;
using TinyHunter.Data.Combat;
using TinyHunter.Data.Monsters;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    [RequireComponent(typeof(MonsterPartBreakSystem))]
    public class MonsterHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private MonsterDefinition definition;
        [SerializeField] private QuestSystem questSystem;
        [SerializeField] private DebugChecklistUI checklist;
        [SerializeField] private GameObject fallbackPickupPrefab;

        private MonsterPartBreakSystem partBreakSystem;
        private float currentHealth;

        private void Awake()
        {
            currentHealth = definition.MaxHealth;
            partBreakSystem = GetComponent<MonsterPartBreakSystem>();
            partBreakSystem.OnPartBroken += OnPartBroken;
        }

        public void TakeHit(float damage, DamageType damageType, string hitPartId = null, Vector3? hitPoint = null)
        {
            if (damageType == definition.Weakness)
            {
                damage *= 1.25f;
            }

            currentHealth -= damage;
            if (!string.IsNullOrEmpty(hitPartId) && partBreakSystem.HasPart(hitPartId))
            {
                partBreakSystem.ApplyPartDamage(hitPartId, damage);
            }

            if (currentHealth <= 0f)
            {
                Defeat();
            }
        }

        private void OnPartBroken(MonsterPartDefinition part)
        {
            if (part.GuaranteedDrop != null)
            {
                SpawnPickup(part.GuaranteedDrop, part.GuaranteedDropAmount);
            }
        }

        private void Defeat()
        {
            if (definition.LootTable != null)
            {
                foreach (var entry in definition.LootTable.Entries)
                {
                    if (Random.value > entry.DropChance || entry.Item == null) continue;
                    int amount = Random.Range(entry.MinAmount, entry.MaxAmount + 1);
                    SpawnPickup(entry.Item, amount);
                }
            }

            checklist?.SetKilledTarget();
            questSystem?.RegisterTargetDefeated(definition.MonsterId);
            Destroy(gameObject);
        }

        private void SpawnPickup(Data.Items.ItemDefinition item, int amount)
        {
            GameObject pickupPrefab = item.WorldPickupPrefab != null ? item.WorldPickupPrefab : fallbackPickupPrefab;
            if (pickupPrefab == null) return;

            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.3f;
            spawnPos.y = transform.position.y + 0.1f;
            var pickupObj = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
            if (pickupObj.TryGetComponent<WorldPickup>(out var pickup))
            {
                pickup.Setup(item, amount);
            }
        }
    }
}
