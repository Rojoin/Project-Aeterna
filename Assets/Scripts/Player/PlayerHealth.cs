using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [Header("Data")]
    [SerializeField]private EntitySO player;
    [SerializeField]private Animator animator;
    [SerializeField]private VoidChannelSO MoveCamera;
     [SerializeField] private CustomSlider healthBar;
    private float currentHealth;
    private float maxHealth;
    private float damage;
    private float speed;
    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    public UnityEvent OnPlayerHurt;

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
        healthBar.FillAmount = currentHealth / maxHealth;
    }
    public float GetHealth() 
    {
        return player.health;
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth += newMaxHealth;
        currentHealth += newMaxHealth;
        healthBar.FillAmount = currentHealth / maxHealth;
    }

    public void HealthPlayer(float AddHealth) 
    {
        if(currentHealth >= maxHealth) 
        {
            currentHealth = maxHealth;
            healthBar.FillAmount = currentHealth;
        }

        else 
        {
            currentHealth += AddHealth;
            healthBar.FillAmount = currentHealth;
        }
    }

[ContextMenu("KillPlayer")]
private void KillPlayer()
{
    ReceiveDamage(1000000);
}
    public void ReceiveDamage(float damage) 
    {
        if (currentHealth <= 0 || currentHealth <= damage) 
        {
            currentHealth = 0;
            AkSoundEngine.SetState("DeathFloorMusic", "Death");
            MoveCamera.RaiseEvent();
            gameObject.SetActive(false);
        }
        else
        {
            currentHealth -= damage;
        }
        OnPlayerHurt.Invoke();
        animator.SetTrigger(IsHurt);
        healthBar.FillAmount = currentHealth / maxHealth;
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