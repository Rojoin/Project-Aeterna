using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public VoidChannelSO AttackChannel;
    public Animator animator;
    public List<AttackSO> combo;
    [SerializeField] private AttackCollision _attackCollider;
    public float timeBetweenCombo = 0.2f;
    public float timeBetweenComboEnd = 0.5f;
    private float lastClickedTime = 0;
    private float lastComboEnd = 0;
    private int comboCounter = 0;
    private float attackTimer;
    private void OnEnable()
    {
        AttackChannel.Subscribe(Attack);
        _attackCollider.OnTriggerEnterObject.AddListener(OnAttackEnter);
        _attackCollider.OnTriggerExitObject.AddListener(OnAttackExit);
    }


    private void OnDisable()
    {
        AttackChannel.Unsubscribe(Attack);
        _attackCollider.OnTriggerEnterObject.RemoveListener(OnAttackEnter);
        _attackCollider.OnTriggerExitObject.RemoveListener(OnAttackExit);
    }


    public void Attack()
    {
        if (Time.time - lastComboEnd > timeBetweenComboEnd && comboCounter <= combo.Count)
        {
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastClickedTime >= timeBetweenCombo)
            {
                animator.runtimeAnimatorController = combo[comboCounter].overrideController;
                _attackCollider.ActivateCollider(combo[comboCounter]);
                animator.Play("Attack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;
                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    private void Update()
    {
        ExitAttack();
    }
//Todo: Make a way to stop attack
    public bool ExitAttack()
    {
        if ( Time.time -lastClickedTime >= combo[comboCounter].attackTime&& 
            animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 0.5f);
            return true;
        }

        return false;
    }

    void EndCombo()
    {
        _attackCollider.StopAttack();
        comboCounter = 0;
        lastComboEnd = Time.time;
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
        if ( combo[comboCounter])
        {
          //  Gizmos.DrawCube(_attackCollider.transform.position + combo[comboCounter].colliderCenter,
          //      combo[comboCounter].colliderSize);
        }
    }
}