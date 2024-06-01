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
        private float currentHealth;
        private float maxHealth;

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
            if (currentHealth <= 0 && currentHealth <= damage)
            {
                currentHealth = 0;
                this.gameObject.SetActive(false);
            }

            else
            {
                currentHealth -= damage;
                OnHit.Invoke();
            }

            healthBar.FillAmount = currentHealth / maxHealth;
        }

        public bool IsDead()
        {
            return currentHealth <= 0;
        }

        private void OnTriggerEnter(Collider other)
        {
        }
    }
}