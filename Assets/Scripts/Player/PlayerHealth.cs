using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [Header("Data")]
    [SerializeField]private EntitySO entity;

    private float currentHealth;
    private float maxHealth;
    private float damage;

    private void Start()
    {
        maxHealth = entity.health;
        currentHealth = maxHealth;
        damage = entity.damage;
    }

    public void SetHealth(float newHealth) 
    {
        currentHealth = newHealth;
    }
    public float GetHealth() 
    {
        return currentHealth;
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

    public void SetDamage(float newDamage) 
    {
        damage = newDamage;
    }
    public float GetDamage() 
    {
        return damage;
    }

    public bool IsDead() 
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
