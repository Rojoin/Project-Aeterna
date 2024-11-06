using UnityEngine;

[CreateAssetMenu(menuName = "Create OkamiSo", fileName = "OkamiSo")]
public class OkamiSo : EntitySO
{
    public float detectionRange;
    public float attackRange;

    public float attackSpeed;
    public float attackTime;
    
    public float chasingMoveSpeed;
    public float attackMoveSpeed;
}