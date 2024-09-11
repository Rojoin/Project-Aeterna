using UnityEngine;

[CreateAssetMenu(menuName = "Create PlayerEntitySO", fileName = "PlayerEntitySO", order = 0)]
public class PlayerEntitySO : EntitySO
{
    private float m_maxHealth;
    public float maxHealth;

    public float movementSpeedDuringAttack;
    public float timeBetweenComboEnd = 2.0f;

    private void OnEnable()
    {
        m_maxHealth = maxHealth;
    }

    private void OnDisable()
    {
        maxHealth = m_maxHealth;
        ResetStacks();
    }
}