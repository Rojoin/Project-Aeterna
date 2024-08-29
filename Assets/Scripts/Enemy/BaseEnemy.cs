using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Enemy
{
    public abstract class BaseEnemy : MonoBehaviour, IHealthSystem
    {
        [FormerlySerializedAs("enemy")] [SerializeField]
        protected EntitySO config;
        [SerializeField] private CustomSlider healthBar;
        [SerializeField] public UnityEvent OnHit;
        [SerializeField] public UnityEvent OnDeath;
        [SerializeField] public UnityEvent<BaseEnemy> OnDeathRemove;
        [SerializeField] public Animator animator;
        private float currentHealth;
        private float maxHealth;
        protected bool canAttack;
        protected float timer;
        public bool isAttacking;
        protected static readonly int AttackAnim = Animator.StringToHash("isAttacking");
        private static readonly int IsIdle = Animator.StringToHash("isIdle");
        private static readonly int Hurt = Animator.StringToHash("isHurt");
        private static readonly int Dead = Animator.StringToHash("isDead");
        private static readonly int Damage = Animator.StringToHash("Damage");
        [SerializeField] float timeAfterDeactivate = 0.50f;

        private void OnEnable()
        {
            Init();
        }

        private void OnValidate()
        {
            ValidateMethod();
        }

        protected abstract void ValidateMethod();

        protected virtual void Init()
        {
            maxHealth = config.health;
            currentHealth = maxHealth;
            healthBar.FillAmount = 1.0f;
            animator.SetFloat(Damage, 1.0f);
            animator.SetTrigger(IsIdle);
        }

        public void SetHealth(float newHealth)
        {
            config.health = newHealth;
        }

        public float GetHealth() => config.health;

        public void SetMaxHealth(float newMaxHealth)
        {
            config.health = newMaxHealth;
        }

        public virtual void ReceiveDamage(float damage)
        {
            if (currentHealth <= 0 || currentHealth <= damage)
            {
                animator.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
                Invoke(nameof(DeactivateObject), timeAfterDeactivate);
            }
            else
            {
                animator.SetTrigger(Hurt);
                currentHealth -= damage;
                OnHit.Invoke();
            }

            float healthNormalize = currentHealth / maxHealth;
            healthBar.FillAmount = healthNormalize;
            animator.SetFloat(Damage, healthNormalize);
        }

        public bool IsDead() => currentHealth <= 0;

        void DeactivateObject()
        {
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
        }
    }
}