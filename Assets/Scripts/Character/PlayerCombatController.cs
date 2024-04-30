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
    public float timeBetweenCombo = 0.2f;
    public float timeBetweenComboEnd = 0.5f;
    private float lastClickedTime = 0;
    private float lastComboEnd = 0;
    private int comboCounter = 0;

    private void OnEnable()
    {
        AttackChannel.Subscribe(Attack);
    }

    private void OnDisable()
    {
        AttackChannel.Unsubscribe(Attack);
    }

    private IEnumerator a()
    {
        yield return null;
    }

    public void Attack()
    {
        if (Time.time - lastComboEnd > timeBetweenComboEnd && comboCounter <= combo.Count)
        {
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastClickedTime >= timeBetweenCombo)
            {
                animator.runtimeAnimatorController = combo[comboCounter].overrideController;

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

    public bool ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f &&
            animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 1);
            return true;
        }

        return false;
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}