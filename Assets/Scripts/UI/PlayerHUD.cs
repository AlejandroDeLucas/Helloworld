using TinyHunter.Core.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private TargetLockSystem lockSystem;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider staminaBar;
        [SerializeField] private Text targetText;

        private void OnEnable()
        {
            if (playerStats == null) return;
            playerStats.OnHealthChanged += UpdateHealth;
            playerStats.OnStaminaChanged += UpdateStamina;
        }

        private void OnDisable()
        {
            if (playerStats == null) return;
            playerStats.OnHealthChanged -= UpdateHealth;
            playerStats.OnStaminaChanged -= UpdateStamina;
        }

        private void Update()
        {
            if (targetText == null || lockSystem == null) return;
            targetText.text = lockSystem.CurrentTarget == null ? "Target: None" : $"Target: {lockSystem.CurrentTarget.name}";
        }

        private void UpdateHealth(float current, float max)
        {
            if (healthBar != null) healthBar.value = max <= 0f ? 0f : current / max;
        }

        private void UpdateStamina(float current, float max)
        {
            if (staminaBar != null) staminaBar.value = max <= 0f ? 0f : current / max;
        }
    }
}
