﻿using System.Collections;
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
        public bool FinalRoom;
        public UnityEvent OnLastEnemyKilled;

        private List<BaseEnemy> enemyList = new List<BaseEnemy>();
        private List<Props> enemyToSpawnList = new List<Props>();

        public VoidChannelSO ActiveSlowTime;

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
            if (!FinalRoom)
            {
                Debug.Log("CallEnemyMusic");
                AkSoundEngine.SetState("DeathFloorMusic", "Combat");
            }

            for (int i = 0; i < enemyToSpawnList.Count; i++)
            {
                Vector3 spawnPosition = enemyToSpawnList[i].propPosition;

                GameObject enemyToInvoke = enemyToSpawnList[i].prop;

                GameObject newEnemy =
                    Instantiate(enemyToInvoke, transform);

                newEnemy.transform.localPosition = spawnPosition + (enemyToInvoke.transform.up *
                                                                    ((enemyToInvoke
                                                                        .transform.localScale.y / 2) + 0.3f));

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
                StartCoroutine(CallExploringMusic());
                CallEndRoom();
            }
        }

        private IEnumerator CallExploringMusic()
        {
            yield return new WaitForSecondsRealtime(1);
            AkSoundEngine.SetState("DeathFloorMusic", "Exploring");
        }

        public void CallEndRoom()
        {
            OnLastEnemyKilled?.Invoke();

            if(GetComponent<RoomBehaviour>().roomType != RoomTypes.START)
            ActiveSlowTime.RaiseEvent();

            roomClear = true;
        }
    }
}