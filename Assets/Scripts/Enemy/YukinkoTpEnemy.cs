using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Projectile;
using ScriptableObjects.Entities;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum YukinkoTpStates
{
    None = 0,
    Idle = 1,
    Attack = 4,
    Teleport = 16
}

public class YukinkoTpEnemy : BaseEnemy, IMovevable
{
    [SerializeField] private BaseProjectile projectile;
    [SerializeField] private SkinnedMeshRenderer meshBody;
    [SerializeField] private SkinnedMeshRenderer meshFace;
    private FSM _fsm;
    private Material materialBody;
    private Material materialFace;
    private ShootingEnemySO enemyConfig;
    private Transform target;
    private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
    private float defenseModeTimer = 0.0f;
    private int hitWhileBlocking = 0;
    private float canEnterDefenseModeTimer = 0.0f;
    private static readonly int IsExitingDefense = Animator.StringToHash("isExitingDefense");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsEnteringDefense = Animator.StringToHash("isEnteringDefense");
    private YukinkoTpStates states = YukinkoTpStates.Idle;
    public UnityEvent OnAttack = new();
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private List<GameObject> hideTeleportGameObjects;
    [SerializeField] private ParticleSystem tpVFX;

    protected override void ValidateMethod()
    {
        if (config == null)
            return;
        if (config.GetType() != typeof(ShootingEnemySO))
        {
            config = null;
        }
        else
        {
            enemyConfig = config as ShootingEnemySO;
        }
    }

    public override void ReceiveDamage(float damage)
    {
        OnHit.Invoke();
        
        Vector3 currentPosition = transform.position;
        Vector3 teleportPosition = GetRandomPointOnNavMesh(currentPosition, enemyConfig.maxEscapeDistance, enemyConfig.minEscapeDistance);
        
        states = YukinkoTpStates.Teleport;
        _navMeshAgent.SetDestination(teleportPosition);
        animator.SetTrigger(IsWalking);

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
        }
        else
        {
            // FBX en la posicion actual
            ParticleSystem particleAtCurrent = Instantiate(tpVFX, currentPosition, Quaternion.identity);
            StartCoroutine(DestroyParticleAfterPlay(particleAtCurrent));
        
            //tp
            ChangeVisibleTpGameObjects(false);
            _navMeshAgent.enabled = false;
            transform.position = teleportPosition;
            _navMeshAgent.enabled = true; 

            // FBX en la nueva posicion
            ParticleSystem particleAtTeleport = Instantiate(tpVFX, teleportPosition, Quaternion.identity);
            StartCoroutine(DestroyParticleAfterPlay(particleAtTeleport));
        
            animator.SetTrigger(Hurt);
            currentHealth -= damage;
            ChangeOnHitColor();
            OnHit.Invoke();
        }

        float healthNormalize = currentHealth / maxHealth;
        healthBar.FillAmount = healthNormalize;

    }

    private IEnumerator DestroyParticleAfterPlay(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.isPlaying);
        Destroy(particle.gameObject);
    }
    
    protected override void Init()
    {
        base.Init();
        enemyConfig = config as ShootingEnemySO;
        projectile.SetSettings(enemyConfig);
        materialBody = meshBody.material;
        materialFace = meshFace.material;
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Update()
    {
        if (!isActivated || IsDead()) return;
        
        _navMeshAgent.speed = enemyConfig.speed;

        if (states.HasFlag(YukinkoTpStates.Teleport))
        {
            if (HasReachedDestination())
            {
                ChangeVisibleTpGameObjects(true);
                states = YukinkoTpStates.Attack;
                attackTimer = enemyConfig.timeBetweenAttacks + 1;
            }
        }
        else
        {
            if (attackTimer > enemyConfig.timeBetweenAttacks)
            {
                states = YukinkoTpStates.Attack;
            }
        }

        if (states.HasFlag(YukinkoTpStates.Attack) && !states.HasFlag(YukinkoTpStates.Teleport))
        {
            DetectEntity();
        }
        else if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
        }
    }

    private bool HasReachedDestination()
    {
        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected void DetectEntity()
    {
        int layerMask = 1 << gameObject.layer;
        layerMask = ~layerMask;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyConfig.attackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && states.HasFlag(YukinkoTpStates.Attack))
            {
                Transform currentObjective = hitCollider.gameObject.transform;
                Vector3 direction = currentObjective.position - transform.position;
                direction.y = 0;
                target = hitCollider.transform;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
                }

                attackTimer = 0.0f;
                animator.SetTrigger(AttackAnim);
                states = states & (~YukinkoTpStates.Attack);
                break;
            }
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
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
        animator.SetTrigger(Dead);
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

    public void SendProjectile()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        target = target.transform;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
        }

        OnAttack.Invoke();
        BaseProjectile baseProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        baseProjectile.SetTarget(target);
        baseProjectile.gameObject.SetActive(true);
    }

    public void Move(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
    {
        StartCoroutine(MoveByAttack(direction, speed, maxTime, curve));
    }

    private IEnumerator MoveByAttack(Vector3 direction, float speed, float maxTime, AnimationCurve curve)
    {
        float timer = 0f;
        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            _navMeshAgent.Move(direction * (speed * curve.Evaluate(timer / maxTime) * Time.deltaTime));
            yield return null;
        }
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float maxDistance, float minDistance)
    {
        NavMeshHit hit;
        Vector3 randomDirection;

        do
        {
            randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += center;
        } while (!NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas) ||
                 Vector3.Distance(center, hit.position) < minDistance);


        return hit.position;
    }

    private void ChangeVisibleTpGameObjects(bool state)
    {
        collider.enabled = state;
        foreach (GameObject o in hideTeleportGameObjects)
        {
            o.SetActive(state);
        }
        
        meshBody.enabled = state;
        meshFace.enabled = state;
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
    }
}