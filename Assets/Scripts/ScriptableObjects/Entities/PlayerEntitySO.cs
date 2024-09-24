using UnityEngine;

[CreateAssetMenu(menuName = "Create PlayerEntitySO", fileName = "PlayerEntitySO", order = 0)]
public class PlayerEntitySO : EntitySO
{
    [Header("Health")]
    public float maxHealth;
    private float m_maxHealth;
    public float healingValue;

    [Header("Attack")]
    public float movementSpeedDuringAttack;
    public float timeBetweenComboEnd = 2.0f;

    public float attackSpeed;

    [Header("Dash")] 
    public float dashSpeed = 4.0f;
    public float dashTimer = 0.5f;
    public float timebetweenDashes = 0.3f;
    public AnimationCurve dashCurve;

    private void OnEnable()
    {
        m_maxHealth = maxHealth;
    }

    private void OnDisable()
    {
        maxHealth = m_maxHealth;
        healingValue = 0;
        dashSpeed = 35f;
        damage = 30;
        speed = 3;
        attackSpeed = 1;
    }
}