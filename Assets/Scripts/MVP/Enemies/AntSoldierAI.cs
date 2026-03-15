using TinyHunter.Core.Player;
using UnityEngine;
using UnityEngine.AI;

namespace TinyHunter.MVP.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    // Hotfix note: player discovery uses FindFirstObjectByType to avoid deprecation warnings.
    public class AntSoldierAI : MonoBehaviour
    {
        private enum State { Idle, Patrol, Chase, Attack, Recover, ReturnToNest }

        [SerializeField] private Transform nestPoint;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float detectionRange = 7f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackDamage = 12f;
        [SerializeField] private float attackCooldown = 1.2f;
        [SerializeField] private float idleDuration = 1.5f;
        [SerializeField] private float chaseTimeout = 5f;

        private PlayerController player;
        private NavMeshAgent agent;
        private State currentState;
        private int patrolIndex;
        private float stateTimer;
        private float timeSinceSeenPlayer;

        private void Awake() => agent = GetComponent<NavMeshAgent>();

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>();
            currentState = State.Idle;
            stateTimer = idleDuration;
        }

        private void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            bool seesPlayer = distanceToPlayer <= detectionRange;
            if (seesPlayer) timeSinceSeenPlayer = 0f;
            else timeSinceSeenPlayer += Time.deltaTime;

            switch (currentState)
            {
                case State.Idle: UpdateIdle(seesPlayer); break;
                case State.Patrol: UpdatePatrol(seesPlayer); break;
                case State.Chase: UpdateChase(distanceToPlayer); break;
                case State.Attack: UpdateAttack(distanceToPlayer); break;
                case State.Recover: UpdateRecover(); break;
                case State.ReturnToNest: UpdateReturnToNest(); break;
            }
        }

        private void UpdateIdle(bool seesPlayer)
        {
            if (seesPlayer) { currentState = State.Chase; return; }
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                currentState = patrolPoints.Length > 0 ? State.Patrol : State.ReturnToNest;
            }
        }

        private void UpdatePatrol(bool seesPlayer)
        {
            if (seesPlayer) { currentState = State.Chase; return; }
            if (patrolPoints.Length == 0) { currentState = State.ReturnToNest; return; }

            agent.SetDestination(patrolPoints[patrolIndex].position);
            if (!agent.pathPending && agent.remainingDistance <= 0.3f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                currentState = State.Idle;
                stateTimer = idleDuration;
            }
        }

        private void UpdateChase(float distanceToPlayer)
        {
            agent.SetDestination(player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.Attack;
                stateTimer = attackCooldown;
                return;
            }

            if (timeSinceSeenPlayer >= chaseTimeout)
            {
                currentState = State.ReturnToNest;
            }
        }

        private void UpdateAttack(float distanceToPlayer)
        {
            stateTimer -= Time.deltaTime;
            if (distanceToPlayer > attackRange)
            {
                currentState = State.Chase;
                return;
            }

            if (stateTimer > 0f) return;

            var stats = player.GetComponent<TinyHunter.Core.Combat.PlayerStats>();
            if (stats != null) stats.ApplyDamage(attackDamage);
            currentState = State.Recover;
            stateTimer = 0.6f;
        }

        private void UpdateRecover()
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                currentState = State.Chase;
            }
        }

        private void UpdateReturnToNest()
        {
            if (nestPoint == null)
            {
                currentState = State.Idle;
                stateTimer = idleDuration;
                return;
            }

            agent.SetDestination(nestPoint.position);
            if (!agent.pathPending && agent.remainingDistance <= 0.5f)
            {
                currentState = State.Idle;
                stateTimer = idleDuration;
            }
        }
    }
}
