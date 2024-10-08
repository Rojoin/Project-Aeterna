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
        private List<Props> enemyToSpawnList = new List<Props>();

        public void OnEnterNewRoom()
        {
            Debug.Log("try spawn Enemies");

            if (!roomClear)
            {
                Debug.Log("Spawn Enemies");
                SpawnEnemies();
            }
        }

        public void SetEnemyRoomStats(LevelRoomPropsSo levelRoomProps)
        {
            foreach (Props currentEnemy in levelRoomProps.EnemyList)
            {
                enemyToSpawnList.Add(currentEnemy);
            }
        }

        private void SpawnEnemies()
        {
            Debug.Log("CallEnemyMusic");
            AkSoundEngine.SetState("DeathFloorMusic", "Combat");

            for (int i = 0; i < enemyToSpawnList.Count; i++)
            {
                Vector3 spawnPosition = enemyToSpawnList[i].propPosition;

                GameObject enemyToInvoke = enemyToSpawnList[i].prop;
                
                GameObject newEnemy =
                    Instantiate(enemyToInvoke, spawnPosition + (enemyToInvoke.transform.up *
                                                                ((enemyToInvoke
                                                                    .transform.localScale.y / 2) + 0.3f)),
                        quaternion.identity, transform);
                
                BaseEnemy newBaseEnemy = newEnemy.GetComponent<BaseEnemy>();

                newBaseEnemy.OnDeathRemove.AddListener(RemoveEnemy);
                enemyList.Add(newBaseEnemy);
            }
            
            EndChamber();
        }

        private void RemoveEnemy(BaseEnemy enemy)
        {
            if (enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
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
            if (enemyList.Count <= 0)
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