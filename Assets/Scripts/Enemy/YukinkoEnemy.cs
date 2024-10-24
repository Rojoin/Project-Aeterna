﻿using System;
using System.Collections;
using Enemy;
using Projectile;
using ScriptableObjects.Entities;
using StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Flags]
public enum YukinkoStates
{
    None = 0,
    Idle = 1,
    CanEnterDefense = 2,
    Attack = 4,
    Defense = 8,
    Run = 16
}

public class YukinkoEnemy : BaseEnemy, IMovevable
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
    private YukinkoStates states = YukinkoStates.Idle;
    public UnityEvent OnAttack = new();
    [SerializeField] private NavMeshAgent _navMeshAgent;

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
        if ((states & YukinkoStates.Defense) == YukinkoStates.Defense)
        {
            OnHit.Invoke();
            isAttacking = true;
            hitWhileBlocking++;
            if (hitWhileBlocking >= enemyConfig.hitsUntilCounterAttack)
            {
                states = states | YukinkoStates.Attack;
                DetectEntity();
                hitWhileBlocking = 0;
                states = states | YukinkoStates.Run;
                _navMeshAgent.SetDestination(GetRandomPointOnNavMesh(transform.position, enemyConfig.maxEscapeDistance,enemyConfig.minEscapeDistance));
                animator.SetTrigger(IsWalking);
                states = states &= ~YukinkoStates.Defense;
                defenseModeTimer = 0;
                isAttacking = false;
            }
        }
        else
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
                animator.SetTrigger(Hurt);
                currentHealth -= damage;
                OnHit.Invoke();
            }
        }

        float healthNormalize = currentHealth / maxHealth;
        healthBar.FillAmount = healthNormalize;
    }

    protected override void Init()
    {
        base.Init();
        enemyConfig = config as ShootingEnemySO;
        projectile.SetSettings(enemyConfig);
        materialBody = meshBody.material;
        materialFace = meshFace.material;
        OnHit.AddListener(ResetDefenseMode);
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void ResetDefenseMode()
    {
        if (states.HasFlag(YukinkoStates.Defense))
        {
            defenseModeTimer = 0.0f;
        }
        else
        {
            states |= YukinkoStates.CanEnterDefense;
        }
    }

    protected void Update()
    {
        if (IsDead()) return;
        _navMeshAgent.speed = enemyConfig.speed;

        if (states.HasFlag(YukinkoStates.Run))
        {
            if (HasReachedDestination())
            {
                states = states &= ~YukinkoStates.Run;
                animator.SetTrigger(IsIdle);
                Debug.Log("Exit Running");
                states = states | YukinkoStates.Idle;
            }
        }

        if (states.HasFlag(YukinkoStates.Attack) && !states.HasFlag(YukinkoStates.Defense))
        {
            DetectEntity();
        }
        else if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
        }

        if (states.HasFlag(YukinkoStates.CanEnterDefense))
        {
            canEnterDefenseModeTimer += Time.deltaTime;
            if (canEnterDefenseModeTimer > enemyConfig.timeUntilBlock)
            {
                animator.SetTrigger(IsEnteringDefense);
                states &= ~YukinkoStates.CanEnterDefense;
                states |= YukinkoStates.Defense;
                canEnterDefenseModeTimer = 0;
            }
        }

        if (states.HasFlag(YukinkoStates.Defense))
        {
            defenseModeTimer += Time.deltaTime;
            if (defenseModeTimer > enemyConfig.timeBetweenAttacks)
            {
                states = states &= ~YukinkoStates.Defense;
                animator.SetTrigger(IsExitingDefense);
                defenseModeTimer = 0;
            }
        }
        else
        {
            if (attackTimer > enemyConfig.timeBetweenAttacks)
            {
                states = states | YukinkoStates.Attack;
            }
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
            if (hitCollider.CompareTag("Player") && states.HasFlag(YukinkoStates.Attack))
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
                states = states & (~YukinkoStates.Attack);
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
        //Maybe change that when is blocking cannot be moved
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

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
    }
}