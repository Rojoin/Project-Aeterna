using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class DummyEnemy : BaseEnemy
    {
        
        public AttackCollision damageCollision;
     
 

        private DummySO enemyConfig;

        protected override void Init()
        {
            base.Init();
            enemyConfig = config as DummySO;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
        }

        
        protected override void ValidateMethod()
        {
            if (config.GetType() != typeof(DummySO))
            {
                config = null;
            }
            else
            {
                enemyConfig = config as DummySO;
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

            if (canAttack)
            {
                DetectEntity();
            }
            else if (!isAttacking)
            {
                attackTimer += Time.deltaTime;
            }

            if (attackTimer > enemyConfig.attackSpeed)
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

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                    }

                    attackTimer = 0.0f;
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
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
        }
    }
}