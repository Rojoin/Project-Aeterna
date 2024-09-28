using UnityEngine;

[CreateAssetMenu(menuName = "Create OkamiSo", fileName = "OkamiSo")]
public class OkamiSo : EntitySO
{
    public int detectionRange;
    public int attackRange;

    public int attackSpeed;
    
    public float chasingMoveSpeed;
    public float attackMoveSpeed;
}