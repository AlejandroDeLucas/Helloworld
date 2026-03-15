using System;
using TinyHunter.Core.Player;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class PlayerStats : MonoBehaviour
    {
        public float MaxHealth = 100f;
        public float MaxStamina = 100f;
        public float Defense = 5f;
        public float MoveSpeed = 2.2f;
        public float DodgeEfficiency = 1f;

        public float CurrentHealth { get; private set; }
        public float CurrentStamina { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        public bool IsBlocking { get; private set; }
        public float GuardReduction { get; private set; } = 0.5f;

        public event Action<float, float> OnHealthChanged;
        public event Action<float, float> OnStaminaChanged;

        [SerializeField] private PlayerAnimationBridge animationBridge;

        private void Awake()
        {
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        }

        public void SetBlocking(bool blocking, float guardReduction)
        {
            IsBlocking = blocking;
            GuardReduction = Mathf.Clamp01(guardReduction);
        }

        public bool SpendStamina(float amount)
        {
            if (CurrentStamina < amount) return false;
            CurrentStamina -= amount;
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
            return true;
        }

        public void RegenerateStamina(float rate)
        {
            CurrentStamina = Mathf.Min(MaxStamina, CurrentStamina + rate * Time.deltaTime);
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        }

        public void ApplyDamage(float rawDamage)
        {
            var preDefense = IsBlocking ? rawDamage * (1f - GuardReduction) : rawDamage;
            var damage = Mathf.Max(1f, preDefense - Defense);
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Max(0f, CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0f)
            {
                animationBridge?.TriggerDeath();
                return;
            }

            animationBridge?.TriggerHit();
        }

        public void RestoreToFull()
        {
            CurrentHealth = MaxHealth;
            CurrentStamina = MaxStamina;
            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        }
    }
}
