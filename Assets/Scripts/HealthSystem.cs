using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("SetUp")]
    [SerializeField]private Entity entity;

    private void Start()
    {
        entity.health = entity.maxHealth;
        entity.damage = entity.maxDamage;
    }

    public void SetMaxHealigh(float newMaxHealth) 
    {
        entity.maxHealth = newMaxHealth;
    }

    public void SetMaxDamage(float newMaxDamage) 
    {
        entity.maxDamage = newMaxDamage;
    }

    public void TakeDamage(float damage) 
    {
        if (entity.health <= 0) 
        {
            entity.health = 0;
        }

        else
        {
            entity.health -= damage;
        }
    }

    public bool isDead() 
    {
        if (entity.health <= 0) 
        {
            return true;
        }

        else 
        {
            return false; 
        }
    }
}