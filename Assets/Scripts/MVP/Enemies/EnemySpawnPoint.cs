using System.Collections.Generic;
using UnityEngine;

namespace TinyHunter.MVP.Enemies
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [Header("Spawn Setup")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnAnchors;
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private bool infiniteSpawns;
        [SerializeField] private int totalSpawnLimit = 3;
        [SerializeField] private int maxAlive = 1;
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private bool randomizePrefabSelection = true;

        [Header("Spawner State")]
        [SerializeField] private bool spawnerCanBeDestroyed;
        [SerializeField] private float spawnerHealth = 20f;

        private readonly List<GameObject> aliveEnemies = new();
        private float spawnTimer;
        private int totalSpawned;
        private bool spawningEnabled = true;

        private void Start()
        {
            if (spawnOnStart)
            {
                TrySpawn();
            }
        }

        private void Update()
        {
            if (!spawningEnabled) return;

            PruneDeadEnemies();
            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f) return;

            TrySpawn();
        }

        public void ApplyDamageToSpawner(float amount)
        {
            if (!spawnerCanBeDestroyed || amount <= 0f) return;

            spawnerHealth -= amount;
            if (spawnerHealth <= 0f)
            {
                spawningEnabled = false;
            }
        }

        public void EnableSpawner() => spawningEnabled = true;
        public void DisableSpawner() => spawningEnabled = false;

        private void TrySpawn()
        {
            spawnTimer = spawnInterval;
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
            if (aliveEnemies.Count >= Mathf.Max(1, maxAlive)) return;
            if (!infiniteSpawns && totalSpawned >= totalSpawnLimit) return;

            GameObject prefab = ChooseEnemyPrefab();
            if (prefab == null) return;

            Transform anchor = ChooseSpawnAnchor();
            Vector3 position = anchor != null ? anchor.position : transform.position;
            Quaternion rotation = anchor != null ? anchor.rotation : transform.rotation;

            GameObject instance = Instantiate(prefab, position, rotation);
            aliveEnemies.Add(instance);
            totalSpawned++;
        }

        private GameObject ChooseEnemyPrefab()
        {
            if (!randomizePrefabSelection)
            {
                return enemyPrefabs[0];
            }

            int index = Random.Range(0, enemyPrefabs.Length);
            return enemyPrefabs[index];
        }

        private Transform ChooseSpawnAnchor()
        {
            if (spawnAnchors == null || spawnAnchors.Length == 0) return null;
            int index = Random.Range(0, spawnAnchors.Length);
            return spawnAnchors[index];
        }

        private void PruneDeadEnemies()
        {
            for (int i = aliveEnemies.Count - 1; i >= 0; i--)
            {
                if (aliveEnemies[i] == null)
                {
                    aliveEnemies.RemoveAt(i);
                }
            }
        }
    }
}
