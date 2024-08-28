using ScriptableObjects.Entities;
using UnityEngine;

namespace Enemy
{
    public class ShootingEnemy : BaseEnemy
    {
        [SerializeField] private Projectile.BaseProjectile projectile;
        private ShootingEnemySO enemyConfig;
        
        protected override void ValidateMethod()
        {
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
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.timeBetweenAttacks);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player") && canAttack)
                {
                    Transform currentObjective = hitCollider.gameObject.transform;
                    Vector3 direction = currentObjective.position - transform.position;
                    direction.y = 0;

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
        
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.timeBetweenAttacks);
        }
    }
}