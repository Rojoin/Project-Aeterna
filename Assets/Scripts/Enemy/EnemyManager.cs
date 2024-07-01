using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public bool roomClear = false;
        public UnityEvent OnLastEnemyKilled;
        public ProceduralRoomGeneration proceduralRoomGeneration;

        public GameObject enemyPrefab;
        private int totalEnemies = 0;
        private List<BaseEnemy> Enemies = new List<BaseEnemy>();
        [SerializeField] private int minNumberEnemies;
        [SerializeField] private int maxNumberEnemies;
        [SerializeField] private float enemyMinSpawnDistance;

        public void OnEnterNewRoom()
        {
            if (!roomClear)
            {
                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            int NewEnemyCuantity = Random.Range(minNumberEnemies, maxNumberEnemies);
            totalEnemies = NewEnemyCuantity;

            for (int i = 0; i < NewEnemyCuantity; i++)
            {
                Cell spawnPositionCell =
                    proceduralRoomGeneration.GetRandomCellByType(CellTag.inside, enemyMinSpawnDistance);
                GameObject newEnemy =
                    Instantiate(enemyPrefab, spawnPositionCell.position + (enemyPrefab.transform.up * enemyPrefab
                        .transform.localScale.y / 2), quaternion.identity, transform);

                Enemies.Add(newEnemy.GetComponent<BaseEnemy>());
            }

            foreach (BaseEnemy e in Enemies)
            {
                e.OnDeath.AddListener(DeletePlayer);
            }
        }

        private void DeletePlayer()
        {
            totalEnemies--;
            if (totalEnemies <= 0)
            {
                foreach (BaseEnemy e in Enemies)
                {
                    e.OnDeath.RemoveListener(DeletePlayer);
                }

                CallEndRoom();
            }
        }

        public void CallEndRoom()
        {
            OnLastEnemyKilled.Invoke();

            roomClear = true;
        }

        private void OnDestroy()
        {
            foreach (BaseEnemy e in Enemies)
            {
                e.OnDeath.RemoveListener(DeletePlayer);
            }
        }
    }
}