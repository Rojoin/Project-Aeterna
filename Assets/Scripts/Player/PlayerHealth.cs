using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [Header("Data")]
    [SerializeField] private PlayerEntitySO player;
    [SerializeField] private Animator animator;
    [SerializeField] private VoidChannelSO MoveCamera;
    [SerializeField] private CustomSlider healthBar;
    [SerializeField] private SelectCardMenu selectCardMenu;

    private float currentHealth;
    private float maxHealth;
    private float damage;
    private float speed;
    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    public UnityEvent OnPlayerHurt;
    private bool isInvencible;

    private void Start()
    {
        player.health = player.maxHealth;
        maxHealth = player.maxHealth;
        currentHealth = player.health;
    }

    private void UpdatePlayerStacks()
    {
        player.maxHealth = maxHealth;
        player.health = currentHealth;
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

    public void HealthPlayer()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            healthBar.FillAmount = currentHealth;
        }

        else
        {
            currentHealth += player.healingValue;
            healthBar.FillAmount = currentHealth;
        }

        UpdatePlayerStacks();
        selectCardMenu.ShowSelectCardMenu(false);
    }

    [ContextMenu("KillPlayer")]
    private void KillPlayer()
    {
        ReceiveDamage(1000000);
    }

    public void ReceiveDamage(float damage)
    {
        if (isInvencible)
        {
            return;
        }
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

        UpdatePlayerStacks();
    }

    public void SetDamage(float newDamage)
    {
        player.damage = newDamage;
    }

    public float GetDamage()
    {
        return player.damage;
    }

    public void SetInvincibility(bool value)
    {
         isInvencible = value;
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
            UpdatePlayerStacks();
            return true;
        }

        else
        {
            return false;
        }
    }
}