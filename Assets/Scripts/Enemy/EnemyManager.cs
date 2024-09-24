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
        
        private List<(BaseEnemy, Vector3)> enemyList = new List<(BaseEnemy, Vector3)>();
        private List<BaseEnemy> enemySpawnedList = new List<BaseEnemy>();

        public void OnEnterNewRoom()
        {
            Debug.Log("try spawn Enemies");

            if (!roomClear)
            {
                Debug.Log("Spawn Enemies");
                SpawnEnemies();
            }
        }

        public void SetEnemyRoomStats(List<(BaseEnemy, Vector3)> newEnemyList)
        {
            enemyList = newEnemyList;
        }

        private void SpawnEnemies()
        {
            Debug.Log("CallEnemyMusic");
            AkSoundEngine.SetState("DeathFloorMusic", "Combat");

            for (int i = 0; i < enemyList.Count; i++)
            {
                BaseEnemy newEnemy =
                    Instantiate(enemyList[i].Item1, enemyList[i].Item2 + (enemyList[i].Item1.transform.up *
                                                                          ((enemyList[i].Item1
                                                                              .transform.localScale.y / 2) + 0.3f)),
                        quaternion.identity, transform);

                
                
                newEnemy.OnDeathRemove.AddListener(RemoveEnemy);
                enemySpawnedList.Add(newEnemy);
                
            }
        }

        private void RemoveEnemy(BaseEnemy enemy)
        {
            if (enemySpawnedList.Contains(enemy))
            {
                enemySpawnedList.Remove(enemy);
                enemy.OnDeathRemove.RemoveListener(RemoveEnemy);
                
                EndChamber();
            }
            else
            {
                Debug.LogError("Enemy not finded to remove");
            }
        }

        private void EndChamber()
        {
            if (enemySpawnedList.Count <= 0)
            {
                Debug.Log("CallExploringMusic");
                AkSoundEngine.SetState("DeathFloorMusic", "Exploring");
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