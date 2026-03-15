using System.Collections;
using TinyHunter.Core.Combat;
using TinyHunter.UI.Panels;
using UnityEngine;

namespace TinyHunter.Core.Respawn
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private PlayerStats stats;
        [SerializeField] private RespawnController respawnController;
        [SerializeField] private PauseMenuUI deathScreen;
        [SerializeField] private float respawnDelay = 2f;

        private bool handled;

        private void Update()
        {
            if (stats == null || handled || !stats.IsDead) return;
            handled = true;
            if (deathScreen != null)
            {
                deathScreen.ShowDeathState();
            }

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);
            respawnController.ReturnToHubOnDeath();
            handled = false;
        }
    }
}
