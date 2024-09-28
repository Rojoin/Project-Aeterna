using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class FollowEnemy : BaseEnemy
    {
        public AttackCollision damageCollision;

        private OkamiSo enemyConfig;
        private bool chasing = false;
        private float currentMovementSpeed;

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

            DetectEntity();

            if (canAttack)
            {
                AttackEntity();
            }
            else if (!isAttacking)
            {
                attackTimer += Time.deltaTime;
            }

            // if (!canAttack)
            // {
            //     currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            // }

            if (attackTimer > enemyConfig.attackSpeed)
            {
                canAttack = true;
            }
        }

        protected void DetectEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.detectionRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player") && canAttack)
                {
                    Transform currentObjective = hitCollider.gameObject.transform;
                    _navMeshAgent.SetDestination(currentObjective.position);
                    chasing = true;
                    break;
                }
            }
        }

        public void AttackEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.attackRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player") && canAttack)
                {
                    Transform currentObjective = hitCollider.gameObject.transform;
                    currentMovementSpeed = enemyConfig.attackMoveSpeed;
                    Vector3 direction = currentObjective.position - transform.position;
                    direction.y = 0;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                    }

                    attackTimer = 0.0f;
                    animator?.SetTrigger(AttackAnim);
                    canAttack = false;
                    break;
                }
            }
        }

        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
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