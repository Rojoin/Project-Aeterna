﻿using System.Collections;
using System.Collections.Generic;
using CustomChannels;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public bool roomClear = false;
        public bool FinalRoom;
        public UnityEvent OnLastEnemyKilled;
        public PlayerPortraitChannelSO ChangePortrait;
        public Vector3ChannelSO SpawnParticleChannel;

        private List<BaseEnemy> enemyList = new List<BaseEnemy>();
        private List<Props> enemyToSpawnList = new List<Props>();

        public VoidChannelSO ActiveSlowTime;

        public void OnEnterNewRoom()
        {
            if (!roomClear)
            {
                SpawnEnemies();
                ChangePortrait.RaiseEvent(PlayerPortraitStates.InBattle);
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
                newBaseEnemy.Init();
                newBaseEnemy.DeactivateModelEnemy();
                newBaseEnemy.OnDeathRemove.AddListener(RemoveEnemy);
                enemyList.Add(newBaseEnemy);
            }

            EndChamber();
        }

        public void ActivateEnemies(float timeToActivate)
        {
            StartCoroutine(ActivateEnemiesTimer(timeToActivate));
        }

        private IEnumerator ActivateEnemiesTimer(float timeToActivate)
        {
            foreach (BaseEnemy currentEnemy in enemyList)
            {
                currentEnemy.ActivateModelEnemy(timeToActivate);
            }

            yield return new WaitForSecondsRealtime(timeToActivate);

            foreach (BaseEnemy currentEnemy in enemyList)
            {
                currentEnemy.ActivateEnemy();
            }
        }

        private void RemoveEnemy(BaseEnemy enemy)
        {
            if (enemyList.Contains(enemy))
            {
                enemyList.Remove(enemy);
                enemy.OnDeathRemove.RemoveListener(RemoveEnemy);


                EndChamber(enemy);
            }
            else
            {
                Debug.LogError("Enemy not finded to remove");
            }
        }

        private void EndChamber(BaseEnemy enemy = null)
        {
            if (enemyList.Count <= 0)
            {
                SpawnParticleChannel.RaiseEvent(enemy.transform.position);
                AkSoundEngine.PostEvent("NewCardAdded", gameObject);
                Debug.Log("CallExploringMusic");
                StartCoroutine(CallExploringMusic());

                CallEndRoom();
            }
        }

        private IEnumerator CallExploringMusic()
        {
            yield return new WaitForSecondsRealtime(1);
            AkSoundEngine.SetState("DeathFloorMusic", "Exploring");
            ChangePortrait.RaiseEvent(PlayerPortraitStates.Normal);
        }

        public void CallEndRoom()
        {
            OnLastEnemyKilled?.Invoke();

            if (GetComponent<RoomBehaviour>().roomType != RoomTypes.START)
                ActiveSlowTime.RaiseEvent();

            roomClear = true;
        }
    }
}