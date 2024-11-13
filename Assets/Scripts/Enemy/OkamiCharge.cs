using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class OkamiCharge : BaseEnemy, IMovevable
    {
        enum OkamiStates
        {
            Searching,
            Chasing,
            Orbit,
            Preparing,
            Attack,
            Dead
        }


        public AttackCollision damageCollision;

        private OkamiChargeSo enemyConfig;
        private OkamiStates currentState = OkamiStates.Searching;
        private float currentMovementSpeed;
        private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
        [SerializeField] private SkinnedMeshRenderer meshBody;
        [SerializeField] private SkinnedMeshRenderer meshFace;
        private Transform playerPosition;
        private float attackTimerlife = 0;
        private float orbitTimerlife = 0;
        private float rotationSpeed = 4;
        private Material materialBody;
        private Material materialFace;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private Transform currentObjective;


        protected override void Init()
        {
            base.Init();
            enemyConfig = config as OkamiChargeSo;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            materialBody = meshBody.material;
            materialFace = meshFace.material;
            animator.SetTrigger(IsIdle);
        }


        protected override void ValidateMethod()
        {
            if (config.GetType() != typeof(OkamiChargeSo))
            {
                config = null;
            }
            else
            {
                enemyConfig = config as OkamiChargeSo;
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
            attackTimer += Time.deltaTime;
            switch (currentState)
            {
                case OkamiStates.Searching:
                    DetectEntity();
                    break;
                case OkamiStates.Chasing:
                    Move();
                    ChasingEntity();
                    break;
                case OkamiStates.Orbit:
                    OrbitPlayer();
                    break;
                case OkamiStates.Preparing:
                    OrientateFace(playerPosition.position);
                    PrepareAttack();
                    break;
                case OkamiStates.Attack:
                    WaitAfterAttack();
                    break;
                case OkamiStates.Dead:
                    break;
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
                    currentMovementSpeed = enemyConfig.chasingMoveSpeed;
                    animator.SetTrigger(IsWalking);
                    break;
                }
            }
        }

        private void OrbitPlayer()
        {
            orbitTimerlife += Time.deltaTime;

            if (orbitTimerlife >= enemyConfig.timeUntilAttack)
            {
                currentState = OkamiStates.Preparing;
                orbitTimerlife = 0;
            }
            else
            {
                Vector3 orbitPosition = playerPosition.position + new Vector3(Mathf.Sin(Time.time * enemyConfig.orbitSpeed), 0,
                                            Mathf.Cos(Time.time * enemyConfig.orbitSpeed)) * enemyConfig.orbitRadius;

                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(orbitPosition);

                OrientateFace(orbitPosition);
            }
        }

        private void ChasingEntity()
        {
            _navMeshAgent.isStopped = false;

            if (Vector3.Distance(transform.position, playerPosition.position) <= enemyConfig.orbitRadius &&
                attackTimer > enemyConfig.attackSpeed)
            {
                currentState = OkamiStates.Orbit;
                _navMeshAgent.isStopped = true;
            }
        }

        private void PrepareAttack()
        {
            attackTimer = 0;
            currentState = OkamiStates.Attack;
            AttackEntity();
        }

        private void OrientateFace(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        private void Move()
        {
            _navMeshAgent.speed = currentMovementSpeed;
            _navMeshAgent.SetDestination(playerPosition.position);
        }

        public void AttackEntity()
        {
            animator?.SetTrigger(AttackAnim);
            AkSoundEngine.PostEvent("Okami_Attack", gameObject);
        }

        private void WaitAfterAttack()
        {
            attackTimerlife -= Time.deltaTime;
            if (attackTimerlife < 0)
            {
                attackTimerlife = enemyConfig.delayAfterAttack;
                currentState = OkamiStates.Searching;
            }
            else if (attackTimerlife > enemyConfig.timeUntilAttackIsLockOn)
            {
                OrientateFace(playerPosition.position);
            }
            else
            {
                _navMeshAgent.isStopped = false;
                currentMovementSpeed = enemyConfig.attackMoveSpeed;
                Move();
            }
        }

        private void DelayUntilChase()
        {
            currentState = OkamiStates.Chasing;
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
                healthBar.gameObject.SetActive(false);
                currentState = OkamiStates.Dead;
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