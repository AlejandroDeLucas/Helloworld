using System.Collections;
using TinyHunter.Core.Inventory;
using TinyHunter.Core.Quest;
using TinyHunter.Data.Combat;
using TinyHunter.Data.Monsters;
using TinyHunter.MVP.Enemies;
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
        [SerializeField] private EnemyAnimationBridge animationBridge;
        [SerializeField] private float destroyDelayAfterDeath = 1.5f;

        private MonsterPartBreakSystem partBreakSystem;
        private ITemporaryInvulnerabilityProvider temporaryInvulnerabilityProvider;
        private float currentHealth;
        private bool isDefeated;

        private void Awake()
        {
            currentHealth = definition.MaxHealth;
            partBreakSystem = GetComponent<MonsterPartBreakSystem>();
            partBreakSystem.OnPartBroken += OnPartBroken;
            if (animationBridge == null) animationBridge = GetComponent<EnemyAnimationBridge>();
            temporaryInvulnerabilityProvider = GetComponent<ITemporaryInvulnerabilityProvider>();
        }

        public void TakeHit(float damage, DamageType damageType, string hitPartId = null, Vector3? hitPoint = null)
        {
            if (isDefeated) return;
            if (temporaryInvulnerabilityProvider != null && temporaryInvulnerabilityProvider.IsInvulnerable) return;
            if (damageType == definition.Weakness)
            {
                damage *= 1.25f;
            }

            currentHealth -= damage;
            if (currentHealth > 0f)
            {
                animationBridge?.PlayHitReaction();
            }

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
            if (isDefeated) return;
            isDefeated = true;
            animationBridge?.SetDying();

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
            StartCoroutine(DestroyAfterDelay());
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

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(destroyDelayAfterDeath);
            Destroy(gameObject);
        }
    }
}
