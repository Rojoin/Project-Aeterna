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


    private void Awake()
    {
        if (_attackCollider == null)
        {
            _attackCollider = GetComponent<BoxCollider>();
            _attackCollider.enabled = false;
        }
    }


    public void ToggleCollider(bool state)
    {
        _attackCollider.enabled = state;
    }

    public void SetColliderParams(Vector3 colliderCenter, Vector3 colliderSize)
    {
        _attackCollider.center = colliderCenter;
        _attackCollider.size = colliderSize;
    }


    public void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterObject.Invoke(other.gameObject);
    }
    public bool OnEndAttack()
    {
        return false;
    }

    public void OnTriggerExit(Collider other)
    {
        OnTriggerExitObject.Invoke(other.gameObject);
    }
}