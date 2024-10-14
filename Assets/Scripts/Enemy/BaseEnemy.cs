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
        [SerializeField]  protected CustomSlider healthBar;
        [SerializeField] public UnityEvent OnHit;
        [SerializeField] public UnityEvent OnDeath;
        [SerializeField] public UnityEvent<BaseEnemy> OnDeathRemove;
        [SerializeField] public Animator animator;
        [SerializeField] public BoxCollider collider;
        protected float currentHealth;
        protected float maxHealth;
        protected bool canAttack;
        protected float attackTimer;
        public bool isAttacking;
        protected static readonly int AttackAnim = Animator.StringToHash("isAttacking");
        protected static readonly int IsIdle = Animator.StringToHash("isIdle");
        protected static readonly int Hurt = Animator.StringToHash("isHurt");
        protected static readonly int Dead = Animator.StringToHash("isDead");
        protected static readonly int Damage = Animator.StringToHash("Damage");
        [SerializeField] float timeAfterDeactivate = 0.50f;
        [SerializeField] protected float disappearSpeed = 5.0f;
        private void OnEnable()
        {
            Init();
            collider = GetComponent<BoxCollider>();
            OnDeath.AddListener(DeathBehaviour);
        }

        private void OnDisable()
        {
            OnDeath.RemoveAllListeners();
            OnHit.RemoveAllListeners();
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
            animator?.SetFloat(Damage, 1.0f);
            animator?.SetTrigger(IsIdle);
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
                animator?.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
                OnDeathRemove.Invoke(this);
                Invoke(nameof(DeathBehaviour), timeAfterDeactivate);
            }
            else
            {
                animator?.SetTrigger(Hurt);
                currentHealth -= damage;
                OnHit.Invoke();
            }

            float healthNormalize = currentHealth / maxHealth;
            healthBar.FillAmount = healthNormalize;
            animator?.SetFloat(Damage, healthNormalize);
        }

        public bool IsDead() => currentHealth <= 0;

        public virtual void DeathBehaviour()
        {
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
        }
    }
}