using System;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private Vector2ChannelSO playerMovement;
        [SerializeField] private VoidChannelSO attackChannel;
        [SerializeField] private VoidChannelSO SpecialAttack;
        [SerializeField] private VoidChannelSO DashChannel;
        [SerializeField] private VoidChannelSO OnTutorialComplete;
 

        [SerializeField] private Image moveIcon;
        [SerializeField] private Image dashIcon;
        [SerializeField] private Image attackIcon;
        [SerializeField] private Image specialIcon;
        [SerializeField] private Sprite completedSprite;

        [SerializeField]  private DummyEnemy _enemy;
        public UnityEvent OnTutorialFinished;
        public int count = 0;


        private void OnEnable()
        {
            
            _enemy.canReceiveDamage = false;
            playerMovement?.Subscribe(OnTutorialCompletedAction);
           attackChannel?.Subscribe(OnAttackCompletedAction);
           SpecialAttack?.Subscribe(OnSpecialCompletedAction);
           DashChannel?.Subscribe(OnDashCompletedAction);
           count = 0; 
           _enemy.OnDeath?.AddListener(OnEnemyDeath);
        }

        private void OnTutorialCompletedAction(Vector2 movementValue = default)
        {
            playerMovement.Unsubscribe(OnTutorialCompletedAction);
            moveIcon.sprite = completedSprite;
            CheckTutorialCondition();

        }     
        private void OnDashCompletedAction()
        {
            DashChannel.Unsubscribe(OnDashCompletedAction);
            dashIcon.sprite = completedSprite;
            CheckTutorialCondition();

        }  
        private void OnSpecialCompletedAction()
        {
            SpecialAttack.Unsubscribe(OnSpecialCompletedAction);
            specialIcon.sprite = completedSprite;
            CheckTutorialCondition();

        } 
        private void OnAttackCompletedAction()
        {
            attackChannel.Unsubscribe(OnAttackCompletedAction);
            attackIcon.sprite = completedSprite;
            CheckTutorialCondition();
        }

        private void OnDisable()
        {

        }

        public void OnEnemyDeath()
        {
            _enemy.OnDeath.RemoveListener(OnEnemyDeath);
            OnTutorialFinished.Invoke();
            OnTutorialComplete.RaiseEvent();
        }

        public void CheckTutorialCondition()
        {
            count++;
            if (count >= 4)
            {
                _enemy.canReceiveDamage = true; 
            }
        }
    }
}