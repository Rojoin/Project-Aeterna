using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class OkamiEnemy : BaseEnemy, IMovevable
    {
        enum OkamiStates
        {
            Searching,
            Chasing,
            Preparing,
            Attack
        }


        public AttackCollision damageCollision;

        private OkamiSo enemyConfig;
        private OkamiStates currentState = OkamiStates.Searching;
        private float currentMovementSpeed;
        private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
        [SerializeField] private SkinnedMeshRenderer meshBody;
        [SerializeField] private SkinnedMeshRenderer meshFace;
        private Transform playerPosition;
        private float attackTimerlife = 0;
        private Material materialBody;
        private Material materialFace;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private Transform currentObjective;


        protected override void Init()
        {
            base.Init();
            enemyConfig = config as OkamiSo;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            materialBody = meshBody.material;
            materialFace = meshFace.material;
            animator.SetTrigger(IsIdle);
        }


        protected override void ValidateMethod()
        {
            if (config.GetType() != typeof(OkamiSo))
            {
                config = null;
            }
            else
            {
                enemyConfig = config as OkamiSo;
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
            _navMeshAgent.speed = currentMovementSpeed;

            switch (currentState)
            {
                case OkamiStates.Searching:
                    DetectEntity();
                    break;
                case OkamiStates.Chasing:
                    Move();
                    ChasingEntity();
                    break;
                case OkamiStates.Preparing:
                    OrientateFace();
                    PrepareAttack();
                    break;
                case OkamiStates.Attack:
                    Move();
                    AttackEntity();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void DetectEntity()
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.detectionRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    currentObjective = hitCollider.gameObject.transform;
                    Vector3 direction = currentObjective.position - transform.position;
                    direction.y = 0;

                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                    }

                    playerPosition = hitCollider.gameObject.transform;
                    currentState = OkamiStates.Chasing;
                    animator.SetTrigger(IsWalking);
                    break;
                }
            }
        }

        private void ChasingEntity()
        {
            if (Vector3.Distance(transform.position, playerPosition.position) <= enemyConfig.attackRange)
            {
                currentState = OkamiStates.Preparing;
            }
        }

        private void PrepareAttack()
        {
            if (attackTimer > enemyConfig.attackSpeed)
            {
                attackTimer = 0;
                currentState = OkamiStates.Attack;
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        private void OrientateFace()
        {
            Vector3 direction = currentObjective.position - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
            }
        }

        private void Move()
        {
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            _navMeshAgent.SetDestination(playerPosition.position);
        }

        public void AttackEntity()
        {
            if (Vector3.Distance(transform.position, playerPosition.position) <= 3 ||
                attackTimerlife >= enemyConfig.attackTime)
            {
                animator?.SetTrigger(AttackAnim);
                attackTimerlife = 0;
                AkSoundEngine.PostEvent("Okami_Attack", gameObject);
                currentState = OkamiStates.Chasing;
            }
            else
            {
                attackTimerlife += Time.deltaTime;
            }
        }

        public override void ReceiveDamage(float damage)
        {
            if (currentHealth <= 0 || currentHealth <= damage)
            {
                animator.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
                OnDeathRemove.Invoke(this);
                collider.enabled = false;
                _navMeshAgent.isStopped = true;
            }
            else
            {
                animator?.SetTrigger(Hurt);
                currentHealth -= damage;
                ChangeOnHitColor();
                OnHit.Invoke();
            }

            float healthNormalize = currentHealth / maxHealth;
            healthBar.FillAmount = healthNormalize;
            // animator?.SetFloat(Damage, healthNormalize);
        }

        public override void DeathBehaviour()
        {
            StartCoroutine(OnDeathMaterialAnimation());
        }

        private IEnumerator OnDeathMaterialAnimation()
        {
            float heightValue = materialBody.GetFloat(CutOffHeight);
            float endAnimation = -5.0f;
            materialFace = meshFace.material;
            while (heightValue > endAnimation)
            {
                heightValue -= Time.deltaTime * disappearSpeed;
                materialBody.SetFloat(CutOffHeight, heightValue);
                materialFace.SetFloat(CutOffHeight, heightValue);
                yield return null;
            }

            materialBody.SetFloat(CutOffHeight, heightValue);
            materialFace.SetFloat(CutOffHeight, heightValue);
            gameObject.SetActive(false);
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.detectionRange);
        }

        public void Move(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
        {
            StartCoroutine(MoveByAttack(direction, speed, maxTime, curve));
        }

        private IEnumerator MoveByAttack(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
        {
            _navMeshAgent.isStopped = true;
            float timer = 0f;
            while (timer < maxTime)
            {
                timer += Time.deltaTime;
                _navMeshAgent.Move(direction * (speed * curve.Evaluate(timer / maxTime) * Time.deltaTime));
                yield return null;
            }

            _navMeshAgent.isStopped = false;
        }
    }
}