using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyWave> possibleWaves;
        private EnemyWave currentWave;
        public UnityEvent OnWaveFinished;
        public UnityEvent OnLastEnemyKilled;
        public GameObject enemyPrefab;
        //Todo: make method that recieves the on kill event and when thelist its empty send a message to RoomManager(wait for wnzo to create  irt)
        private List<BaseEnemy> currentEnemies;
        private Room currentRoom;
        private int numberOfEnemies;

        private void OnEnable()
        {
            OnWaveFinished.AddListener(FinishCurrentWave);
        }

        private void OnDisable()
        {
            OnWaveFinished.RemoveAllListeners();
        }

        public void OnEnterNewRoom(Room currentRoom)
        {
            this.currentRoom = currentRoom;
            currentWave = possibleWaves[Random.Range(0, possibleWaves.Count)];
            numberOfEnemies = currentWave.numberOfEnemies;
            
        }

        // public IEnumerator SpawnEnemies()
        // {
        //     
        //     while (expression)
        //     {
        //         var enemy = Instantiate(currentEnemies);
        //         currentEnemies.Add(enemy.GetComponent<BaseEnemy>());
        //         enemy.GetComponent<BaseEnemy>().OnDeathRemove.AddListener(DeleteFromCurrentEnemies);
        //     }
        // }
        // public void SpawnEnemies()
        // {
        //     var enemy = Instantiate(enemyPrefab);
        //     currentEnemies.Add(enemy.GetComponent<BaseEnemy>());
        //     enemy.GetComponent<BaseEnemy>().OnDeathRemove.AddListener(DeleteFromCurrentEnemies);
        // }

        private void DeleteFromCurrentEnemies(BaseEnemy enemy)
        {
            enemy.OnDeathRemove.RemoveListener(DeleteFromCurrentEnemies);
            if (currentEnemies.Remove(enemy))
            {
                Debug.Log($"Deleted enemy {enemy.name}:{enemy.gameObject.GetInstanceID()}");
            }

            if (currentEnemies.Count <= 0)
            {
                OnWaveFinished.Invoke();
            }
        }

        private void FinishCurrentWave()
        {
        }
    }
}