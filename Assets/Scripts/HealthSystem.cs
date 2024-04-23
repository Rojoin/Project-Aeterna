using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField]private Entity entity;

    private float currentHealth;
    private float maxHealth;

    private void Start()
    {
        maxHealth = entity.health;
        currentHealth = maxHealth;
    }

    public void SetHealth(float newHealth) 
    {
        currentHealth = newHealth;
    }

    public void SetMaxHealigh(float newMaxHealth) 
    {
        maxHealth = newMaxHealth;
    }

    public void ReceiveDamage(float damage) 
    {
        if (currentHealth <= 0 && currentHealth <= damage) 
        {
            currentHealth = 0;
        }

        else
        {
            currentHealth -= damage;
        }
    }

    public bool isDead() 
    {
        if (currentHealth <= 0) 
        {
            return true;
        }

        else 
        {
            return false; 
        }
    }
}