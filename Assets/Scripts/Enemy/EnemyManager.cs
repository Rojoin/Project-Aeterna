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
        
        private List<BaseEnemy> enemyList = new List<BaseEnemy>();
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

            for (int i = 0; i < NewEnemyCuantity; i++)
            {
                Vector3 spawnPosition = SpawnPoints[i].position;

                BaseEnemy enemyToInvoke = enemyLevelSo.enemiesList[Random.Range(0, enemyLevelSo.enemiesList.Count)];

                BaseEnemy newEnemy =
                    Instantiate(enemyToInvoke, spawnPosition + (enemyToInvoke.transform.up *
                                                                ((enemyToInvoke
                                                                    .transform.localScale.y / 2) + 0.3f)),
                        quaternion.identity, transform);

                newEnemy.OnDeathRemove.AddListener(RemoveEnemy);
                enemyList.Add(newEnemy);
            }
        }

        private void RemoveEnemy(BaseEnemy enemy)
        {
            Debug.LogError("Enemy not finded");

            if (enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
                enemy.OnDeathRemove.RemoveListener(RemoveEnemy);
                EndChamber();
            }
            else
            {
                Debug.LogError("Enemy not finded");
            }
        }

        private void EndChamber()
        {
            if (enemyList.Count <= 0)
            {
                CallEndRoom();
            }
        }

        public void CallEndRoom()
        {
            OnLastEnemyKilled?.Invoke();
            roomClear = true;
        }
    }
}