using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace StateMachine
{
    public enum PlayerFlags
    {
        Move,
        Attack,
        EndAttack,
        Pause
    }

    public class PlayerFSM : MonoBehaviour
    {
        [SerializeField] protected Animator _playerAnimatorController;
        [SerializeField] protected CharacterController _characterController;
        [SerializeField] protected Vector2ChannelSO OnMoveChannel;
        [SerializeField] protected VoidChannelSO AttackChannel;
        [SerializeField] protected GameSettings gameSettings;
        [SerializeField] protected EntitySO player;
        [SerializeField] private List<AttackSO> comboList;
        [SerializeField] private AttackCollision _attackCollider;
        protected float speed;
        private FSM fsm;
        private Vector2 moveDir;

        private void OnEnable()
        {
            speed = player.speed;
            fsm = new(2, 3);
            int idleState = fsm.AddNewState(new PlayerMoveState(this.gameObject, _playerAnimatorController,
                _characterController, OnMoveChannel, player));
            int attackState = fsm.AddNewState(new PlayerAttackState(ChangeFromEndAttack, this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player, comboList, _attackCollider, AttackChannel, gameSettings));

            fsm.SetTranstions(idleState, PlayerFlags.Attack, attackState);
            fsm.SetTranstions(attackState, PlayerFlags.EndAttack, idleState);
            AttackChannel.Subscribe(ChangeFromAttack);
            fsm.SetDefaultState(idleState);
        }

        private void Update()
        {
            fsm.Update();
        }

        private void ChangeFromAttack()
        {
            fsm.OnTriggerState(PlayerFlags.Attack);
        }

        private void ChangeFromEndAttack()
        {
            fsm.OnTriggerState(PlayerFlags.EndAttack);
        }

        public void ChangeFromPause(bool value)
        {
            PlayerBaseState.isPause = value;
            PlayerBaseState.inputDirection = Vector2.zero;
            _playerAnimatorController.speed = value ? 0 : 1;
            ChangeFromEndAttack();
        }

        private void MoveDirection(Vector2 newMoveDir)
        {
            moveDir = newMoveDir;
        }

        private void OnDisable()
        {
            fsm.OnDestroy();
            AttackChannel.Unsubscribe(ChangeFromAttack);
        }
    }
}