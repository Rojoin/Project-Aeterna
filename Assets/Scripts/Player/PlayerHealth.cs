using CustomChannels;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    private static readonly int CutOffHeight = Shader.PropertyToID("_Cutoff_Height");
    [Header("Data")] [SerializeField] private PlayerEntitySO player;
    [SerializeField] private PlayerPortraitChannelSO ChangePortrait;
   
    [SerializeField] private Animator animator;
    [SerializeField] private BoolChannelSO onDeath;
    [SerializeField] private VoidChannelSO OnHealth;
    [SerializeField] private SkinnedMeshRenderer skin;
    private Material material;
    [SerializeField] private PlayerHealthBar healthBar;
    [SerializeField] private SelectCardMenu selectCardMenu;
    [SerializeField] float rumbleDuration = 0.1f;
    [SerializeField] protected float disappearSpeed = 5.0f;
    [SerializeField] protected ParticleSystem vfxAura;

    [Header("Timer when take damage")] [SerializeField]
    private float freceTimeTimer = 0.3f;

    [Header("Overlay")] 
    [SerializeField] private VoidChannelSO OnHit;
    [SerializeField] private BoolChannelSO OnLowHealth;
    const int healingValue = 100;

    private float currentHealth;
    private float maxHealth;
    private float damage;
    private float speed;
    private static readonly int IsHurt = Animator.StringToHash("isHurt");
    public UnityEvent OnPlayerHurt;
    private bool isInvencible;
    private static readonly int Dead = Animator.StringToHash("isDead");

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
        currentHealth += healingValue + (healingValue * player.healingValue / 100);

        OverlayManager();

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

        vfxAura?.Play();
        healthBar.FillAmount = currentHealth;
    }

    [ContextMenu("KillPlayer")]
    private void TestDead()
    {
        Invoke(nameof(KillPlayer), 0.5f);
    }

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
            OnLowHealth.RaiseEvent(false);
            currentHealth = 0;
            AkSoundEngine.SetState("DeathFloorMusic", "Death");
            isInvencible = true;
            DeathBehaviour();
        }

        else
        {
            currentHealth -= damage;

            // StartCoroutine(SpawnOverlay(0.5f));
            OnHit.RaiseEvent();
            OverlayManager();
            gameObject.StartRumble(player.rumbleBeingHittingDuration, player.rumbleBeingHittingForce);
            gameObject.StartColorChange(material, player.colorshiftDuration);
            StartCoroutine(FreceTime(freceTimeTimer));
            OnPlayerHurt.Invoke();

            if (currentHealth < player.maxHealth / 2)
            {
                ChangePortrait.RaiseEvent(PlayerPortraitStates.LowHealth);
            }

            ChangePortrait.RaiseEvent(PlayerPortraitStates.Hit);

            animator.SetTrigger(IsHurt);
            healthBar.FillAmount = currentHealth / maxHealth;
        }

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

    public void DeathBehaviour()
    {
        animator.SetTrigger(Dead);
        onDeath.RaiseEvent(true);
        StartCoroutine(OnDeathMaterialAnimation());
    }

    private IEnumerator OnDeathMaterialAnimation()
    {
        float heightValue = material.GetFloat(CutOffHeight);
        float endAnimation = -5.0f;
        while (heightValue > endAnimation)
        {
            heightValue -= Time.deltaTime * disappearSpeed;
            material.SetFloat(CutOffHeight, heightValue);
            yield return null;
        }

        material.SetFloat(CutOffHeight, heightValue);
        gameObject.SetActive(false);
    }

    private void OverlayManager()
    {
        OnLowHealth.RaiseEvent(currentHealth <= maxHealth / 2);
    }

    private IEnumerator FreceTime(float time)
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
    }

    private IEnumerator SpawnOverlay(float time)
    {
        yield return new WaitForSeconds(time);
        OverlayManager();
    }
}