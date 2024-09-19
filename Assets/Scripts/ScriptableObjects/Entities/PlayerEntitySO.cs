using UnityEngine;

[CreateAssetMenu(menuName = "Create PlayerEntitySO", fileName = "PlayerEntitySO", order = 0)]
public class PlayerEntitySO : EntitySO
{
    public float movementSpeedDuringAttack;
    public float timeBetweenComboEnd = 2.0f;
    [Header("Dash")] 
    public float dashSpeed = 4.0f;
    public float dashTimer = 0.5f;
    public float timebetweenDashes = 0.3f;
    public AnimationCurve dashCurve;

}