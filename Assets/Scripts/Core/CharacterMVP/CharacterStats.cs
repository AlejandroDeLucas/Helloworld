using System;
using TinyHunter.Data.Characters;
using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class CharacterStats : MonoBehaviour
    {
        [SerializeField] private CharacterStatsDefinition baseDefinition;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float defense = 2f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public float Damage => damage;
        public float AttackSpeed => attackSpeed;
        public float MoveSpeed => moveSpeed;
        public float Defense => defense;
        public bool IsDead => CurrentHealth <= 0f;

        public event Action<float, float> OnHealthChanged;

        private void Awake()
        {
            ApplyDefinitionIfAssigned();
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void ApplyDefinitionIfAssigned()
        {
            if (baseDefinition == null) return;

            maxHealth = Mathf.Max(1f, baseDefinition.MaxHealth);
            damage = Mathf.Max(0f, baseDefinition.Damage);
            attackSpeed = Mathf.Max(0.1f, baseDefinition.AttackSpeed);
            moveSpeed = Mathf.Max(0f, baseDefinition.MoveSpeed);
            defense = Mathf.Max(0f, baseDefinition.Defense);
        }

        public void SetCurrentHealth(float value)
        {
            CurrentHealth = Mathf.Clamp(value, 0f, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Heal(float amount)
        {
            if (amount <= 0f) return;
            SetCurrentHealth(CurrentHealth + amount);
        }

        public void TakeDamage(float rawDamage)
        {
            if (IsDead) return;
            float reduced = Mathf.Max(1f, rawDamage - defense);
            SetCurrentHealth(CurrentHealth - reduced);
        }

        public void ResetToFull()
        {
            SetCurrentHealth(maxHealth);
        }
    }
}
