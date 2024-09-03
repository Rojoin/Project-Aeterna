using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyWave : ScriptableObject
    {
        public List<BaseEnemy> typesOfEnemies;
        public int numberOfEnemies;
        [SerializeField] public float timeBetweenSpawns = 0.3f;
    }
}