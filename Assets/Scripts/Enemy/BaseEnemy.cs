using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Enemy
{
    public abstract class BaseEnemy : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private EntitySO enemy;
        [SerializeField] private CustomSlider healthBar;
        [SerializeField] public UnityEvent OnHit;
        [SerializeField] public UnityEvent OnDeath;
        [FormerlySerializedAs("OnDeathDissapear")] [SerializeField] public UnityEvent<BaseEnemy> OnDeathRemove;
        [SerializeField] private Animator enemyAnimator;
        private float currentHealth;
        private float maxHealth;
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int Hurt = Animator.StringToHash("isHurt");
        private static readonly int Dead = Animator.StringToHash("isDead");
        private static readonly int Damage = Animator.StringToHash("Damage");
        [SerializeField] float timeAfterDeactivate = 0.50f;

        private void OnEnable()
        {
            maxHealth = enemy.health;
            currentHealth = maxHealth;
            healthBar.FillAmount = 1.0f;
            enemyAnimator.SetFloat(Damage,1.0f);
            enemyAnimator.SetTrigger(IsIdle);
        }

        public void SetHealth(float newHealth)
        {
            enemy.health = newHealth;
        }

        public float GetHealth() => enemy.health;

        public void SetMaxHealigh(float newMaxHealth)
        {
            enemy.health = newMaxHealth;
        }

        public virtual void ReceiveDamage(float damage)
        {
            if (currentHealth <= 0 || currentHealth <= damage)
            {
                enemyAnimator.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
                Invoke(nameof(DeactivateObject), timeAfterDeactivate);
            }
            else
            {
                enemyAnimator.SetTrigger(Hurt);
                currentHealth -= damage;
                OnHit.Invoke();
            }

            float healthNormalize = currentHealth / maxHealth;
            healthBar.FillAmount = healthNormalize;
            enemyAnimator.SetFloat(Damage,healthNormalize);
        }

        public bool IsDead()
        {
            return currentHealth <= 0;
        }

        void DeactivateObject()
        {
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
        }
    }
}