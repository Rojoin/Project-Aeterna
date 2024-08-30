using System;
using System.Collections;
using Enemy;
using Projectile;
using ScriptableObjects.Entities;
using UnityEngine;

[Flags]
public enum YukinkoStates
{
    None =0,
    Idle = 1,
    CanEnterDefense = 2,
    Attack = 4,
    Defense = 8
}

public class ShootingEnemy : BaseEnemy
{
    [SerializeField] private BaseProjectile projectile;
    [SerializeField] private SkinnedMeshRenderer meshBody;
    [SerializeField] private SkinnedMeshRenderer meshFace;
    private Material materialBody;
    private Material materialFace;
    private ShootingEnemySO enemyConfig;
    private Transform target;
    private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
    private float defenseModeTimer = 0.0f;
    private float canEnterDefenseModeTimer = 0.0f;
    private static readonly int IsExitingDefense = Animator.StringToHash("isExitingDefense");
    private static readonly int IsEnteringDefense = Animator.StringToHash("isEnteringDefense");
    private YukinkoStates states = YukinkoStates.Idle;


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
        }
        else
        {
            if (currentHealth <= 0 || currentHealth <= damage)
            {
                animator.SetTrigger(Dead);
                currentHealth = 0;
                OnHit.Invoke();
                OnDeath.Invoke();
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
        projectile.SetSettings(enemyConfig);
        materialBody = meshBody.material;
        materialFace = meshFace.material;
        OnHit.AddListener(ResetDefenseMode);
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
                states = states &=  ~YukinkoStates.Defense;
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
            heightValue -= Time.deltaTime * 2;
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
        BaseProjectile baseProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        baseProjectile.SetTarget(target);
        baseProjectile.gameObject.SetActive(true);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyConfig.attackRange);
    }
}