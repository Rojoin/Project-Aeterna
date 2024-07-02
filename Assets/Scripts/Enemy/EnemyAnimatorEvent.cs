using Enemy;
using UnityEngine;

public class EnemyAnimatorEvent : MonoBehaviour
{
    public DummyEnemy _dummyEnemy;
    public void SetAttackState(int value)
    {
        _dummyEnemy.isAttacking = value != 0;
    }
    
}