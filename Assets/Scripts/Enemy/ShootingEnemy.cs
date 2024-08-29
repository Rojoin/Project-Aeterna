using Projectile;
using ScriptableObjects.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Enemy
{
    public class ShootingEnemy : BaseEnemy
    {
        [SerializeField] private Projectile.BaseProjectile projectile;
        private ShootingEnemySO enemyConfig;
        private Transform target;
       private Collider[] _colliders = new Collider[2];


        protected override void ValidateMethod()
        {
            if (config == null) 
                return;
            if (config.GetType() != typeof(ShootingEnemySO))
            {
                config = null;
            }
            else
            {
                enemyConfig = config as ShootingEnemySO;
            }
        }

        protected override void Init()
        {
            base.Init();
            projectile.SetSettings(enemyConfig);
        }

        protected void Update()
        {
            if (IsDead()) return;

            if (canAttack)
            {
                DetectEntity();
            }
            else if (!isAttacking)
            {
                timer += Time.deltaTime;
            }

            if (timer > enemyConfig.timeBetweenAttacks)
            {
                canAttack = true;
            }
        }

        protected void DetectEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.attackRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player") && canAttack)
                {
                    Transform currentObjective = hitCollider.gameObject.transform;
                    Vector3 direction = currentObjective.position - transform.position;
                    direction.y = 0;
                    target = hitCollider.transform;
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                    }

                    timer = 0.0f;
                    animator.SetTrigger(AttackAnim);
                    canAttack = false;
                    break;
                }
            }
        }

        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
        }

        public void SendProjectile()
        {
            BaseProjectile baseProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            baseProjectile.SetTarget(target);
            baseProjectile.gameObject.SetActive(true);
        }
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
        }
    }
}