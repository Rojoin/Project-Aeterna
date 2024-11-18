using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class DummyEnemy : BaseEnemy
    {
        public AttackCollision damageCollision;
        public bool canReceiveDamage = true;
        private DummySO enemyConfig;
        private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
        private float shieldMaxStrength = -0.2f;
        private float shieldMinStrength = 1.2f;
        private static readonly int Dissolve = Shader.PropertyToID("_Dissolve");
        public UnityEvent OnHitShield;
        [SerializeField] private MeshRenderer meshShield;
        private Material materialShield;

        protected override void Init()
        {
            base.Init();
            enemyConfig = config as DummySO;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            materialShield = meshShield.material;
            canReceiveDamage = false;
            StartCoroutine(ActivateShieldAnimation());
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

        public override void ReceiveDamage(float damage)
        {
            if (!canReceiveDamage)
            {
                gameObject.StartColorChange(materialShield, config.colorshiftDuration);
                OnHitShield.Invoke();
                return;
            }

            if (currentHealth <= 0 || currentHealth <= damage)
            {
                animator?.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
                OnDeathRemove.Invoke(this);
                healthBar.gameObject.SetActive(false);
            }
            else
            {
                animator?.SetTrigger(Hurt);
                ChangeOnHitColor();
                currentHealth -= damage;
                OnHit.Invoke();
                LookAtEnemy();
            }

            float healthNormalize = currentHealth / maxHealth;
            healthBar.FillAmount = healthNormalize;
            animator?.SetFloat(Damage, healthNormalize);
        }

        private void LookAtEnemy()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.attackRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    Transform currentObjective = hitCollider.gameObject.transform;
                    Vector3 direction = currentObjective.position - transform.position;
                    direction.y = 0;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                    }
                }
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

        private IEnumerator OnDeathMaterialAnimation()
        {
            float heightValue = material.GetFloat(CutOffHeight);
            float endAnimation = -5.0f;
            animator.SetTrigger(Dead);
            while (heightValue > endAnimation)
            {
                heightValue -= Time.deltaTime * disappearSpeed;
                material.SetFloat(CutOffHeight, heightValue);
                yield return null;
            }

            material.SetFloat(CutOffHeight, heightValue);
            gameObject.SetActive(false);
        }

        public override void DeathBehaviour()
        {
            StartCoroutine(OnDeathMaterialAnimation());
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
        }

        private IEnumerator ActivateShieldAnimation()
        {
            var timer = 0.0f;
            while (timer < enemyConfig.timeUntilShieldFullyAppears)
            {
                timer += Time.deltaTime;
                float dissolveStrengh = Mathf.Lerp(shieldMinStrength, shieldMaxStrength, timer);
                materialShield.SetFloat(Dissolve, dissolveStrengh);
                yield return null;
            }

            materialShield.SetFloat(Dissolve, shieldMaxStrength);
        }

        public void DeactivateShield()
        {
            StartCoroutine(DeactivateShieldAnimation());
        }

        private IEnumerator DeactivateShieldAnimation()
        {
            var timer = 0.0f;
            while (timer < enemyConfig.timeUntilShieldFullyAppears)
            {
                timer += Time.deltaTime;
                float dissolveStrengh = Mathf.Lerp(shieldMaxStrength, shieldMinStrength, timer);
                materialShield.SetFloat(Dissolve, dissolveStrengh);
                yield return null;
            }

            materialShield.SetFloat(Dissolve, shieldMinStrength);
            canReceiveDamage = true;
        }
    }
}