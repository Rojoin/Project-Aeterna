using System.Collections.Generic;
using CustomChannels;
using StateMachine;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [Header("Data")] [SerializeField] private PlayerEntitySO player;
    [SerializeField] private PlayerPortraitChannelSO ChangePortrait;
    [SerializeField] private Animator animator;
    [SerializeField] private VoidChannelSO MoveCamera;
    [SerializeField] private VoidChannelSO OnHealth;
    [SerializeField] private SkinnedMeshRenderer skin;
    private Material material;
    [SerializeField] private PlayerHealthBar healthBar;
    [SerializeField] private SelectCardMenu selectCardMenu;
    [SerializeField] float rumbleDuration = 0.1f;

    const int healingValue = 100;

    private float currentHealth;
    private float maxHealth;
    private float damage;
    private float speed;
    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    public UnityEvent OnPlayerHurt;
    private bool isInvencible;

    private void OnEnable()
    {
        OnHealth.Subscribe(HealthPlayer);
    }

    private void Start()
    {
        player.health = player.maxHealth;
        maxHealth = player.maxHealth;
        currentHealth = player.health;
        ChangePortrait.RaiseEvent(PlayerPortraitStates.Normal);
        material = skin.materials[1];
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
        UpdatePlayerStacks();
        if (player.health > player.maxHealth / 2)
        {
            ChangePortrait.RaiseEvent(PlayerPortraitStates.Normal);
        }
    }

    public void HealthPlayer()
    {
        currentHealth += healingValue + player.healingValue;

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdatePlayerStacks();
        selectCardMenu.ShowSelectCardMenu(false);
        if (player.health > player.maxHealth / 2)
        {
            ChangePortrait.RaiseEvent(PlayerPortraitStates.Normal);
        }

        healthBar.FillAmount = currentHealth;
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

        gameObject.StartRumble(player.rumbleBeingHittingDuration, player.rumbleBeingHittingForce);
        gameObject.StartColorChange(material, player.colorshiftDuration);
        OnPlayerHurt.Invoke();
        if (currentHealth < player.maxHealth / 2)
        {
            ChangePortrait.RaiseEvent(PlayerPortraitStates.LowHealth);
        }

        ChangePortrait.RaiseEvent(PlayerPortraitStates.Hit);

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

    private void OnDisable()
    {
        OnHealth.Unsubscribe(HealthPlayer);
    }
}