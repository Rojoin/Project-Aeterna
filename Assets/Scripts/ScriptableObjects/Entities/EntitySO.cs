using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
public class EntitySO : ScriptableObject
{
    private float m_health;
    public float health;

    private float m_damage;
    public float damage;

    private float m_speed;
    public float speed;

    public bool isDead;

    public void ResetStacks() 
    {
        damage = m_damage;
        health = m_health;
        speed = m_speed;
        isDead = false;
    }

    private void OnEnable()
    {
        m_damage = damage;
        m_health = health;
        m_speed = speed;
        isDead = false;

        Debug.Log("enable");
    }
}
