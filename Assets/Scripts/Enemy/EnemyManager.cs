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

        private int totalEnemies = 0;
        private List<BaseEnemy> Enemies = new List<BaseEnemy>();
        [SerializeField] private Transform[] SpawnPoints;
        private EnemyLevelSO enemyLevelSo;
        
        public void OnEnterNewRoom() 
        {
            Debug.Log("try spawn Enemies");

            if (!roomClear)
            {
                Debug.Log("Spawn Enemies");
                SpawnEnemies();
            }
        }

        public void SetEnemyRoomStats(EnemyLevelSO levelSo)
        {
            enemyLevelSo = levelSo;
        }

        private void SpawnEnemies()
        {
            int NewEnemyCuantity = Random.Range(enemyLevelSo.minNumberEnemies, enemyLevelSo.maxNumberEnemies);
            totalEnemies = NewEnemyCuantity;

            for (int i = 0; i < NewEnemyCuantity; i++)
            {
                Vector3 spawnPosition = SpawnPoints[i].position;

                GameObject enemyToInvoke = enemyLevelSo.enemiesList[Random.Range(0, enemyLevelSo.enemiesList.Count)];

                GameObject newEnemy =
                    Instantiate(enemyToInvoke, spawnPosition + (enemyToInvoke.transform.up *
                                                                ((enemyToInvoke
                                                                    .transform.localScale.y / 2) + 0.3f)),
                        quaternion.identity, transform);
                newEnemy.transform.Rotate(Vector3.up, 180);
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