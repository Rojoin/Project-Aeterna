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
            Attack,
            Dead
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


        public override void Init()
        {
            base.Init();
            enemyConfig = config as OkamiSo;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            currentMovementSpeed = enemyConfig.chasingMoveSpeed;
            materialBody = meshBody.material;
            materialFace = meshFace.material;
            animator.SetTrigger(IsIdle);
            attackTimerlife = enemyConfig.delayAfterAttack;

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

        public override void ActivateModelEnemy(float endTime)
        {
            base.ActivateModelEnemy(endTime);
            StartCoroutine(OnSpawnMaterialAnimation(endTime));
        }

        public override void DeactivateModelEnemy()
        {
            materialBody.SetFloat(CutOffHeight, -heightValue);
            materialFace.SetFloat(CutOffHeight, -heightValue);
        }

        private IEnumerator OnSpawnMaterialAnimation(float endTime)
        {
            float animation = 0;
            float startAnim = -heightValue;
            float timer = 0;
            float endAnimation = heightValue;
            materialFace = meshFace.material;
            while (timer < endTime)
            {
                timer += Time.deltaTime;
                animation = Mathf.Lerp(startAnim, endAnimation, timer / endTime);
                materialBody.SetFloat(CutOffHeight, animation);
                materialFace.SetFloat(CutOffHeight, animation);
                yield return null;
            }

            materialBody.SetFloat(CutOffHeight, endAnimation);
            materialFace.SetFloat(CutOffHeight, endAnimation);
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
            if (!isActivated || IsDead()) return;
            
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
                case OkamiStates.Preparing:
                    OrientateFace();
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
                    // animator.SetTrigger(IsWalking);
                    break;
                }
            }
        }

        private void ChasingEntity()
        {
            _navMeshAgent.isStopped = false;

            if (Vector3.Distance(transform.position, playerPosition.position) <= enemyConfig.attackRange &&
                attackTimer > enemyConfig.attackSpeed)
            {
                currentState = OkamiStates.Preparing;
                _navMeshAgent.isStopped = true;
            }
        }

        private void PrepareAttack()
        {
            attackTimer = 0;
            currentState = OkamiStates.Attack;
            AttackEntity();
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
            _navMeshAgent.speed = currentMovementSpeed;
            _navMeshAgent.SetDestination(playerPosition.position);
        }

        public void AttackEntity()
        {
            if (currentState == OkamiStates.Dead)
                return;
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
                OrientateFace();
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
            float animation = heightValue;
            float endAnimation = -5.0f;
            materialFace = meshFace.material;
            animator.SetTrigger(Dead);
            while (animation > endAnimation)
            {
                animation -= Time.deltaTime * disappearSpeed;
                materialBody.SetFloat(CutOffHeight, animation);
                materialFace.SetFloat(CutOffHeight, animation);
                yield return null;
            }

            materialBody.SetFloat(CutOffHeight, animation);
            materialFace.SetFloat(CutOffHeight, animation);
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