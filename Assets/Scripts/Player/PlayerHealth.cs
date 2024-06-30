using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [Header("Data")]
    [SerializeField]private EntitySO player;
    [SerializeField]private Animator animator;

    private float currentHealth;
    private float maxHealth;
    private float damage;
    private float speed;
    private static readonly int IsHurt = Animator.StringToHash("isHurt");

    private void Start()
    {
        maxHealth = player.health;
        currentHealth = maxHealth;
        damage = player.damage;
        speed = player.speed;
    }

    public void SetHealth(float newHealth) 
    {
        player.health = newHealth;
    }
    public float GetHealth() 
    {
        return player.health;
    }

    public void SetMaxHealigh(float newMaxHealth) 
    {
        player.health = newMaxHealth;
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
        animator.SetTrigger(IsHurt);
    }

    public void SetDamage(float newDamage) 
    {
        player.damage = newDamage;
    }
    public float GetDamage() 
    {
        return player.damage;
    }

    public void SetSpeed(float newSpeed)
    {
        player.speed = newSpeed;
    }
    public float GetSpeed()
    {
        return player.speed;
    }
    public float GetMaxSpeed()
    {
        return player.maxSpeed;
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

    private void OnDisable()
    {
        player.ResetStacks();
    }
}
