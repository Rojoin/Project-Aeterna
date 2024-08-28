using UnityEngine;

[CreateAssetMenu(menuName = "Create PlayerEntitySO", fileName = "PlayerEntitySO", order = 0)]
public class PlayerEntitySO : EntitySO
{
    public float movementSpeedDuringAttack;
    public float timeBetweenComboEnd = 2.0f;
}