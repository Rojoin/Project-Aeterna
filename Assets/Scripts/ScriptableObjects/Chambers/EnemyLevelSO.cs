using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Level", menuName = "Enemy Level Stats")]
public class EnemyLevelSO : ScriptableObject
{
    public List<BaseEnemy> enemiesList;
    public int minNumberEnemies;
    public int maxNumberEnemies;
}