using TinyHunter.Core.Combat;
using TinyHunter.Core.Player;
using UnityEngine;
using UnityEngine.AI;

namespace TinyHunter.MVP.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    // Hotfix note: player discovery uses FindFirstObjectByType to avoid deprecation warnings.
    public class SpiderAmbushAI : MonoBehaviour
    {
        private enum State { Hidden, AmbushReady, Lunge, Recover, RetreatToWeb }

        [SerializeField] private Transform webRetreatPoint;
        [SerializeField] private float ambushTriggerDistance = 5f;
        [SerializeField] private float lungeDistance = 2.2f;
        [SerializeField] private float lungeDamage = 20f;
        [SerializeField] private float venomDamagePerTick = 3f;
        [SerializeField] private int venomTicks = 3;

        private PlayerController player;
        private PlayerStats playerStats;
        private NavMeshAgent agent;
        private State currentState;
        private float stateTimer;

        private void Awake() => agent = GetComponent<NavMeshAgent>();

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
            if (player != null) playerStats = player.GetComponent<PlayerStats>();
            currentState = State.Hidden;
        }

        private void Update()
        {
            if (player == null || playerStats == null) return;

            float distance = Vector3.Distance(transform.position, player.transform.position);
            switch (currentState)
            {
                case State.Hidden: UpdateHidden(distance); break;
                case State.AmbushReady: UpdateAmbushReady(distance); break;
                case State.Lunge: UpdateLunge(distance); break;
                case State.Recover: UpdateRecover(); break;
                case State.RetreatToWeb: UpdateRetreat(); break;
            }
        }

        private void UpdateHidden(float distance)
        {
            if (distance <= ambushTriggerDistance)
            {
                currentState = State.AmbushReady;
                stateTimer = 0.5f;
            }
        }

        private void UpdateAmbushReady(float distance)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f && distance <= ambushTriggerDistance)
            {
                currentState = State.Lunge;
            }
        }

        private void UpdateLunge(float distance)
        {
            agent.SetDestination(player.transform.position);
            if (distance > lungeDistance) return;

            playerStats.ApplyDamage(lungeDamage);
            for (int i = 0; i < venomTicks; i++)
            {
                playerStats.ApplyDamage(venomDamagePerTick);
            }

            currentState = State.Recover;
            stateTimer = 1.2f;
        }

        private void UpdateRecover()
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                currentState = State.RetreatToWeb;
            }
        }

        private void UpdateRetreat()
        {
            if (webRetreatPoint == null)
            {
                currentState = State.Hidden;
                return;
            }

            agent.SetDestination(webRetreatPoint.position);
            if (!agent.pathPending && agent.remainingDistance <= 0.4f)
            {
                currentState = State.Hidden;
            }
        }
    }
}
