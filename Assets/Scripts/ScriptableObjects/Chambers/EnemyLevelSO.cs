using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Level", menuName = "Enemy Level Stats")]
public class EnemyLevelSO : ScriptableObject
{
    public List<GameObject> enemiesList;
    public int minNumberEnemies;
    public int maxNumberEnemies;
}