using Enemy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyAnimatorEvent : MonoBehaviour
{
    public BaseEnemy baseEnemy;
    public UnityEvent OnAttack;

    public void SetAttackState(int value)
    {
        baseEnemy.isAttacking = value != 0;
    }

    public void ActivateAttack() => OnAttack.Invoke();
}