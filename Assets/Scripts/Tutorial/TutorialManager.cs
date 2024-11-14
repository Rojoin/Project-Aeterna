using System;
using Enemy;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private Vector2ChannelSO playerMovement;
        [SerializeField] private IntChannelSO attackChannel;
        [SerializeField] private BoolChannelSO SpecialAttack;

        private DummyEnemy _enemy;
        public UnityEvent OnTutorialFinished;


        private void OnEnable()
        {
            _enemy.canReceiveDamage = false;
            playerMovement.Subscribe(OnTutorialCompletedAction);
        }

        private void OnTutorialCompletedAction(Vector2 playerMovement = default)
        {
            
        }
        private void OnDisable()
        {
            playerMovement.Unsubscribe(OnTutorialCompletedAction);
        }

        public void OnEnemyDeath()
        {
            OnTutorialFinished.Invoke();
        }
    }
}