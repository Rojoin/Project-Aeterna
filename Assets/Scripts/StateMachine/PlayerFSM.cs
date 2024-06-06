using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace StateMachine
{    public enum PlayerFlags
    {
        Move,
        Attack,
        EndAttack
    }

    public class PlayerFSM : MonoBehaviour
    {
        [SerializeField] protected Animator _playerAnimatorController;
        [SerializeField] protected CharacterController _characterController;
        [SerializeField] protected Vector2ChannelSO OnMoveChannel;
        [SerializeField] protected VoidChannelSO AttackChannel;
        [SerializeField] protected EntitySO player;
        [SerializeField] private List<AttackSO> comboList;
        [SerializeField] private AttackCollision _attackCollider;
        protected float speed;
        private FSM playerFsm;
        private Vector2 moveDir;

        private void OnEnable()
        {
            speed = player.speed;
            playerFsm = new(2, 3);
            int idleState = playerFsm.AddNewState(new PlayerMoveState(this.gameObject, _playerAnimatorController,
                _characterController, OnMoveChannel, speed, player));
            int attackState = playerFsm.AddNewState(new PlayerAttackState(ChangeFromEndAttack,this.gameObject, _playerAnimatorController,
                _characterController, OnMoveChannel, speed, player, comboList, _attackCollider,AttackChannel));

            playerFsm.SetTranstions(idleState, PlayerFlags.Attack, attackState);
            playerFsm.SetTranstions(attackState, PlayerFlags.EndAttack, idleState);
            AttackChannel.Subscribe(ChangeFromAttack);
            playerFsm.SetDefaultState(idleState);
            AttackChannel.Subscribe(ChangeFromAttack);
        }

        private void Update()
        {
            playerFsm.Update();
        }
        private void ChangeFromAttack()
        {
            playerFsm.OnTriggerState(PlayerFlags.Attack);
        }
        private void ChangeFromEndAttack()
        {
            playerFsm.OnTriggerState(PlayerFlags.EndAttack);
        }

        private void MoveDirection(Vector2 newMoveDir)
        {
            moveDir = newMoveDir;
        }
        private void OnDisable()
        {
            playerFsm.OnDestroy();
            AttackChannel.Unsubscribe(ChangeFromAttack);
        }
    }
}