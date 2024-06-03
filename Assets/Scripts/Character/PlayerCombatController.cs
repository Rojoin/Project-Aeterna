using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombatController : MonoBehaviour
{
    public VoidChannelSO AttackChannel;
    public Animator animator;
    public List<AttackSO> combo;
    [SerializeField] private AttackCollision _attackCollider;
    [SerializeField] UnityEvent OnAttack;
    public float timeBetweenCombo = 0.2f;
    public float timeBetweenComboEnd = 0.5f;
    private float lastClickedTime = 0;
    private float lastComboEnd = 0;
    private int comboCounter = 0;
    private float attackTimer;
    [SerializeField] private float attackRadius = 5.0f;
    private GameObject previousTarget;
    private Coroutine _attacking;
    private float timeUntilAttackEnds;
    private float timeUntilStart;
    public bool isMouseActive;

    private void OnEnable()
    {
        AttackChannel.Subscribe(Attack);
        _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
        _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
        lastComboEnd = timeBetweenComboEnd;
    }


    private void OnDisable()
    {
        AttackChannel.Unsubscribe(Attack);
        _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
        _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
    }
//Todo: Fix attack logic not entering the correct state.
//Probably doe to bad implementation of if statement
//Check better wave to do combos

    private void Attack()
    {
        if (Time.time - lastComboEnd > timeBetweenComboEnd && comboCounter <= combo.Count)
        {
            if (Time.time - lastClickedTime >= timeBetweenCombo)
            {
                ActivateCollider(combo[comboCounter]);
                animator.CrossFade(combo[comboCounter].animationName, 0.25f, 0, 0);
                comboCounter++;
                if (GetComponent<PlayerMovement>().GetRotatedMoveDir().magnitude <= 0.1f)
                {
                    CheckTarget();
                }

                lastClickedTime = Time.time;
                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }
    
    //Maybe have a playerSettings and change a flag
    //Todo: Change to check first the distance between the object 
    private void CheckTarget()
    {
        if (isMouseActive)
        {
            Vector3 characterPosition = transform.position;
            Vector3 mouseScreenPosition = Input.mousePosition;
            
            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            Plane plane = new Plane(Vector3.up, characterPosition);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(distance);
                
                Vector3 direction = mouseWorldPosition - characterPosition;
                direction.y = 0;
                GetComponent<PlayerMovement>().Rotate(direction);
            }
        }
        else
        {
            Collider[] possibleTargets =
                Physics.OverlapSphere(transform.position, attackRadius, LayerMask.GetMask($"Target"));
            Vector3 direction;
            if (possibleTargets.Length > 0)
            {
                GameObject currentTarget = null;
                float minDistance = 999;
                currentTarget = possibleTargets[0].gameObject;

                foreach (Collider target in possibleTargets)
                {
                    float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                    if (distanceToTarget < minDistance)
                    {
                        currentTarget = target.gameObject;
                    }

                    direction = (currentTarget.transform.position - transform.position).normalized;
                    direction = new Vector3(direction.x, 0, direction.z);
                    GetComponent<PlayerMovement>().Rotate(direction);
                }
            }
            else
            {
                Debug.Log("No target in the area.");
            }
        }
    }


    private void Update()
    {
        //ExitAttack();
    }

//Todo: Make a way to stop attack
// Maybe change send a signal in the corroutine if ends

    public void StopAttack()
    {
        if (_attacking != null)
        {
            StopCoroutine(_attacking);
        }

        _attackCollider.ToggleCollider(false);
    }

    private IEnumerator AttackCorroutine()
    {
        timeUntilStart = combo[comboCounter].timeUntilStart;
        var timeAfterComboEnds = combo[comboCounter].timeUntilEnd;
        timeUntilAttackEnds = combo[comboCounter].attackTime - timeUntilStart - timeAfterComboEnds;
        yield return new WaitForSeconds(timeUntilStart);
        OnAttack.Invoke();
        _attackCollider.ToggleCollider(true);
        yield return new WaitForSeconds(timeUntilAttackEnds);
        _attackCollider.ToggleCollider(false);
        yield return new WaitForSeconds(timeAfterComboEnds);
        EndCombo();
        yield break;
    }

    public void ActivateCollider(AttackSO attacksParams)
    {
        StopAttack();
        _attackCollider.SetColliderParams(attacksParams.colliderCenter, attacksParams.colliderSize);
        timeUntilStart = attacksParams.timeUntilStart;
        timeUntilAttackEnds = attacksParams.attackTime - timeUntilStart;

        _attacking = StartCoroutine(AttackCorroutine());
    }

    void EndCombo()
    {
        StopAttack();
        comboCounter = 0;
        lastComboEnd = Time.time;
        animator.CrossFade("NormalStatus", 0.25f, 0, 0);
    }

    private void OnAttackExit(GameObject other)
    {
        if (!other.CompareTag("Player") && other.TryGetComponent<IHealthSystem>(out var healthSystem))
        {
            Debug.Log("Exit attack.");
        }
    }

    private void OnAttackEnter(GameObject other)
    {
        if (!other.CompareTag("Player") && other.TryGetComponent<IHealthSystem>(out var healthSystem))
        {
            Debug.Log("Enter attack.");
            healthSystem.ReceiveDamage(combo[comboCounter].damage);
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position, attackRadius);
        // if (comboList[comboCounter])
        {
            //  Gizmos.DrawCube(_attackCollider.transform.position + comboList[comboCounter].colliderCenter,
            //      comboList[comboCounter].colliderSize);
        }
    }
}