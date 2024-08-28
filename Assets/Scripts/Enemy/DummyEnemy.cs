using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class DummyEnemy : BaseEnemy
    {
        public AttackCollision damageCollision;
        private static readonly int AttackAnim = Animator.StringToHash("isAttacking");
        protected bool canAttack;
        protected float timer;
        public bool isAttacking;
        protected override void Init()
        {
            base.Init();
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
        }

        private void OnDisable()
        {
            damageCollision.OnTriggerEnterObject.RemoveListener(DamageEnemy);
        }

        private void DamageEnemy(GameObject arg0)
        {
            if (arg0.TryGetComponent<IHealthSystem>(out var entity))
            {
                entity.ReceiveDamage(enemy.damage);
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
                timer += Time.deltaTime;
            }

            if (timer > enemy.attackSpeed)
            {
                canAttack = true;
            }
        }

        protected void DetectEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemy.attackRange);
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
            Gizmos.DrawWireSphere(transform.position, enemy.attackRange);
        }
    }
}