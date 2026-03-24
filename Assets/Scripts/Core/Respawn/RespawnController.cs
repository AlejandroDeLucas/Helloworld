using TinyHunter.Core.Combat;
using TinyHunter.Core.Flow;
using UnityEngine;

namespace TinyHunter.Core.Respawn
{
    public class RespawnController : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private PlayerStats playerStats;

        public void RespawnPlayer(GameObject player)
        {
            if (respawnPoint != null)
            {
                player.transform.position = respawnPoint.position;
                player.transform.rotation = respawnPoint.rotation;
            }

            playerStats?.RestoreToFull();
        }

        public void ReturnToHubOnDeath()
        {
            SceneFlowController.Instance.ReturnToHub();
        }
    }
}
