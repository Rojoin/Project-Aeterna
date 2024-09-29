using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class OkamiEnemy : BaseEnemy
    {
        enum OkamiStates
        {
            Searching,
            Chasing,
            Preparing,
            Attack
        }


        public AttackCollision damageCollision;

        private OkamiSo enemyConfig;
        private OkamiStates currentState = OkamiStates.Searching;
        private float currentMovementSpeed;

        private Transform playerPosition;
        private float attackTimerlife = 0;

        [SerializeField] private NavMeshAgent _navMeshAgent;

        protected override void Init()
        {
            base.Init();
            enemyConfig = config as OkamiSo;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
        }


        protected override void ValidateMethod()
        {
            if (config.GetType() != typeof(OkamiSo))
            {
                config = null;
            }
            else
            {
                enemyConfig = config as OkamiSo;
            }
        }

        private void OnDisable()
        {
            damageCollision.OnTriggerEnterObject.RemoveListener(DamageEnemy);
        }

        private void DamageEnemy(GameObject arg0)
        {
            if (arg0.TryGetComponent<IHealthSystem>(out var entity))
            {
                entity.ReceiveDamage(config.damage);
            }
        }

        protected void Update()
        {
            if (IsDead()) return;

            _navMeshAgent.speed = currentMovementSpeed * Time.deltaTime;

            switch (currentState)
            {
                case OkamiStates.Searching:
                    DetectEntity();
                    break;
                case OkamiStates.Chasing:
                    ChasingEntity();
                    break;
                case OkamiStates.Preparing:
                    PreapreAttack();
                    break;
                case OkamiStates.Attack:
                    AttackEntity();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void DetectEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.detectionRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    playerPosition = hitCollider.gameObject.transform;
                    _navMeshAgent.SetDestination(playerPosition.position);
                    currentState = OkamiStates.Chasing;
                    break;
                }
            }
        }

        private void ChasingEntity()
        {
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            _navMeshAgent.SetDestination(playerPosition.position);

            if (Vector3.Distance(transform.position, playerPosition.position) <= enemyConfig.attackRange)
            {
                currentState = OkamiStates.Preparing;
            }
        }

        private void PreapreAttack()
        {
            _navMeshAgent.isStopped = true;
            if (attackTimer > enemyConfig.attackSpeed)
            {
                attackTimer = 0;
                _navMeshAgent.isStopped = false;
                currentState = OkamiStates.Attack;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        public void AttackEntity()
        {
            currentMovementSpeed = enemyConfig.attackMoveSpeed;
            _navMeshAgent.SetDestination(playerPosition.position);

            if (Vector3.Distance(transform.position, playerPosition.position) <= 3 || attackTimerlife >= enemyConfig.attackTime)
            {
                animator?.SetTrigger(AttackAnim);
                attackTimerlife = 0;
                currentState = OkamiStates.Chasing;
            }
            else
            {
                attackTimerlife += Time.deltaTime;
            }
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.detectionRange);
        }
    }
}