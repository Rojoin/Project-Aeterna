using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class OkamiEnemy : BaseEnemy, IMovevable
    {
        private enum OkamiStates
        {
            Searching,
            Chasing,
            Preparing,
            Attacking
        }

        public AttackCollision damageCollision;

        private OkamiSo enemyConfig;
        private OkamiStates currentState = OkamiStates.Searching;
        private float currentMovementSpeed;
        private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");

        [SerializeField] private SkinnedMeshRenderer meshBody;
        [SerializeField] private SkinnedMeshRenderer meshFace;
        [SerializeField] private NavMeshAgent navMeshAgent;

        private Transform playerPosition;
        private Transform currentObjective;
        private Material materialBody;
        private Material materialFace;
        
        private float attackLifeTimer = 0f;

        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        protected override void Init()
        {
            base.Init();
            enemyConfig = config as OkamiSo;
            damageCollision.OnTriggerEnterObject.AddListener(DamageEnemy);
            materialBody = meshBody.material;
            materialFace = meshFace.material;
            animator.SetTrigger(IsIdle);
            SetMovementSpeed(enemyConfig.chasingMoveSpeed);
        }

        protected override void ValidateMethod()
        {
            enemyConfig = config as OkamiSo;
            if (enemyConfig == null) config = null;
        }

        private void OnDisable() => damageCollision.OnTriggerEnterObject.RemoveListener(DamageEnemy);

        private void DamageEnemy(GameObject target)
        {
            if (target.TryGetComponent<IHealthSystem>(out var entity))
            {
                entity.ReceiveDamage(config.damage);
            }
        }

        private void Update()
        {
            if (IsDead()) return;

            navMeshAgent.speed = currentMovementSpeed;
            switch (currentState)
            {
                case OkamiStates.Searching:
                    DetectEntity();
                    break;
                case OkamiStates.Chasing:
                    ChaseEntity();
                    break;
                case OkamiStates.Preparing:
                    PrepareAttack();
                    break;
                case OkamiStates.Attacking:
                    ExecuteAttack();
                    break;
            }
        }

        private void DetectEntity()
        {
            animator.SetTrigger(IsWalking);
            var hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.detectionRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    SetTarget(hitCollider.transform);
                    currentState = OkamiStates.Chasing;
                    break;
                }
            }
        }

        private void SetTarget(Transform target)
        {
            playerPosition = target;
            currentObjective = target;
            navMeshAgent.SetDestination(playerPosition.position);
            FaceTarget();
        }

        private void ChaseEntity()
        {
            SetMovementSpeed(enemyConfig.chasingMoveSpeed);
            navMeshAgent.SetDestination(playerPosition.position);

            if (Vector3.Distance(transform.position, playerPosition.position) <= enemyConfig.attackRange)
            {
                currentState = OkamiStates.Preparing;
            }
        }

        private void PrepareAttack()
        {
            if (attackTimer >= enemyConfig.attackSpeed)
            {
                attackTimer = 0;
                currentState = OkamiStates.Attacking;
                navMeshAgent.isStopped = false;
                FaceTarget();
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        private void ExecuteAttack()
        {
            SetMovementSpeed(enemyConfig.attackMoveSpeed);
            navMeshAgent.SetDestination(playerPosition.position);

            if (attackLifeTimer >= enemyConfig.attackTime)
            {
                animator.SetTrigger(AttackAnim);
                attackLifeTimer = 0;
                AkSoundEngine.PostEvent("Okami_Attack", gameObject);
                currentState = OkamiStates.Chasing;
            }
            else
            {
                attackLifeTimer += Time.deltaTime;
            }
        }

        public override void ReceiveDamage(float damage)
        {
            currentHealth -= damage;
            OnHit.Invoke();
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
            else
            {
                animator.SetTrigger(Hurt);
                healthBar.FillAmount = currentHealth / maxHealth;
            }
        }

        private void HandleDeath()
        {
            animator.SetTrigger(Dead);
            OnDeath.Invoke();
            OnDeathRemove.Invoke(this);
            navMeshAgent.isStopped = true;
            collider.enabled = false;
            StartCoroutine(OnDeathMaterialAnimation());
        }

        private IEnumerator OnDeathMaterialAnimation()
        {
            float heightValue = materialBody.GetFloat(CutOffHeight);
            const float endAnimation = -5.0f;
            while (heightValue > endAnimation)
            {
                heightValue -= Time.deltaTime * disappearSpeed;
                materialBody.SetFloat(CutOffHeight, heightValue);
                materialFace.SetFloat(CutOffHeight, heightValue);
                yield return null;
            }

            gameObject.SetActive(false);
        }

        private void FaceTarget()
        {
            Vector3 direction = currentObjective.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    Time.deltaTime * 5);
            }
        }

        private void SetMovementSpeed(float speed) => currentMovementSpeed = speed;

        public void Move(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
        {
            StartCoroutine(MoveByAttack(direction, speed, maxTime, curve));
        }

        private IEnumerator MoveByAttack(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
        {
            navMeshAgent.isStopped = true;
            float timer = 0f;
            while (timer < maxTime)
            {
                timer += Time.deltaTime;
                navMeshAgent.Move(direction * (speed * curve.Evaluate(timer / maxTime) * Time.deltaTime));
                yield return null;
            }

            navMeshAgent.isStopped = false;
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyConfig.detectionRange);
        }
    }
}