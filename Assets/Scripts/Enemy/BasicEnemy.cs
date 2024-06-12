using System;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class BasicEnemy : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private EntitySO enemy;
        [SerializeField] private CustomSlider healthBar;
        [SerializeField] private UnityEvent OnHit;
        [SerializeField] private Animator enemyAnimator;
        private float currentHealth;
        private float maxHealth;
        private static readonly int Hurt = Animator.StringToHash("isHurt");
        private static readonly int Dead = Animator.StringToHash("isDead");
        const float timeAfterDeactivate = 0.30f;

        private void Start()
        {
            maxHealth = enemy.health;
            currentHealth = maxHealth;
            healthBar.FillAmount = 1.0f;
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

        public void ReceiveDamage(float damage)
        {
            if (currentHealth <= 0 || currentHealth <= damage)
            {
                enemyAnimator.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                Invoke(nameof(DeactivateObject), timeAfterDeactivate);
            }
            else
            {
                enemyAnimator.SetTrigger(Hurt);
                currentHealth -= damage;
                OnHit.Invoke();
            }

            healthBar.FillAmount = currentHealth / maxHealth;
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