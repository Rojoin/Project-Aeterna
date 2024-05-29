using System;
using System.Collections;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

public class AttackCollision : MonoBehaviour
{
    [SerializeField] private BoxCollider _attackCollider;
    public UnityEvent<GameObject> OnTriggerEnterObject = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> OnTriggerExitObject = new UnityEvent<GameObject>();
    private Coroutine _attacking;
    private float timeUntilAttackEnds;
     private float timeUntilStart;

    private void Awake()
    {
        if (_attackCollider == null)
        {
            _attackCollider = GetComponent<BoxCollider>();
            _attackCollider.enabled = false;
        }
    }

    public void ActivateCollider(AttackSO attacksParams)
    {
        StopAttack();
        _attackCollider.center = attacksParams.colliderCenter;
        _attackCollider.size = attacksParams.colliderSize;
        timeUntilStart = attacksParams.timeUntilStart;
        timeUntilAttackEnds = attacksParams.attackTime - timeUntilStart;

        _attacking = StartCoroutine(AttackCorroutine());
    }

    private IEnumerator AttackCorroutine()
    {
        yield return new WaitForSeconds(timeUntilStart);
        _attackCollider.enabled = true;
        yield return new WaitForSeconds(timeUntilAttackEnds);
        _attackCollider.enabled = false;
        yield break;
    }

    public void StopAttack()
    {
        if (_attacking != null)
        {
            StopCoroutine(_attacking);
        }

        _attackCollider.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterObject.Invoke(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        OnTriggerExitObject.Invoke(other.gameObject);
    }
}