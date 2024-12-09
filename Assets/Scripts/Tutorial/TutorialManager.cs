using Enemy;
using UnityEngine;
using UnityEngine.Events;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private Vector2ChannelSO playerMovement;
        [SerializeField] private VoidChannelSO attackChannel;
        [SerializeField] private VoidChannelSO SpecialAttack;
        [SerializeField] private VoidChannelSO DashChannel;
        [SerializeField] private VoidChannelSO OnTutorialComplete;


        [SerializeField] private Animator moveObjective;
        [SerializeField] private Animator dashObjective;
        [SerializeField] private Animator attackObjective;
        [SerializeField] private Animator specialObjective;


        [SerializeField] private DummyEnemy _enemy;
        public UnityEvent OnTutorialFinished;
        public int count = 0;


        private void OnEnable()
        {
            playerMovement?.Subscribe(OnTutorialCompletedAction);
            attackChannel?.Subscribe(OnAttackCompletedAction);
            SpecialAttack?.Subscribe(OnSpecialCompletedAction);
            DashChannel?.Subscribe(OnDashCompletedAction);
            count = 0;
            _enemy.OnDeath?.AddListener(OnEnemyDeath);
        }

        private void OnDestroy()
        {
            playerMovement?.Unsubscribe(OnTutorialCompletedAction);
            attackChannel?.Unsubscribe(OnAttackCompletedAction);
            SpecialAttack?.Unsubscribe(OnSpecialCompletedAction);
            DashChannel?.Unsubscribe(OnDashCompletedAction);
        }

        private void OnTutorialCompletedAction(Vector2 movementValue = default)
        {
            playerMovement.Unsubscribe(OnTutorialCompletedAction);
            moveObjective.enabled = true;
            CheckTutorialCondition();
        }

        private void OnDashCompletedAction()
        {
            DashChannel.Unsubscribe(OnDashCompletedAction);
            dashObjective.enabled = true;
            AkSoundEngine.PostEvent("ObjectiveCompleted", gameObject);
            CheckTutorialCondition();
        }

        private void OnSpecialCompletedAction()
        {
            SpecialAttack.Unsubscribe(OnSpecialCompletedAction);
            specialObjective.enabled = true;
            AkSoundEngine.PostEvent("ObjectiveCompleted", gameObject);
            CheckTutorialCondition();
        }

        private void OnAttackCompletedAction()
        {
            attackChannel.Unsubscribe(OnAttackCompletedAction);
            attackObjective.enabled = true;
            AkSoundEngine.PostEvent("ObjectiveCompleted", gameObject);
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
                _enemy.DeactivateShield();
            }
        }
    }
}