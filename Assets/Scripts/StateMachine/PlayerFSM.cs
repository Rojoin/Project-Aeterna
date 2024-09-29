using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace StateMachine
{
    public enum PlayerFlags
    {
        Move,
        Attack,
        EndAttack,
        OnDashStart,
        OnDashEnd,
        OnSpecialAttack,
        Pause
    }

    public enum PlayerStates
    {
        Move,
        Attack,
        SpecialAttack,
        Dash
    }

    public class PlayerFSM : MonoBehaviour
    {
        [SerializeField] protected Animator _playerAnimatorController;
        [SerializeField] protected CharacterController _characterController;
        [SerializeField] protected Vector2ChannelSO OnMoveChannel;
        [SerializeField] protected VoidChannelSO AttackChannel;
        [SerializeField] protected VoidChannelSO SpecialAttackChannel;
        [SerializeField] protected VoidChannelSO DashChannel;
        [SerializeField] protected GameSettings gameSettings;
        [SerializeField] protected PlayerEntitySO player;
        [SerializeField] private List<AttackSO> comboList;
        [SerializeField] private AttackSO specialAttack;
        [SerializeField] private AttackCollision _attackCollider;
        [SerializeField] private UnityEvent onMove;
        [SerializeField] private UnityEvent onDash;
        [SerializeField] private UnityEvent onEndDash;
        [SerializeField] private ParticleSystem specialAttackVFX;
        protected float speed;
        private FSM fsm;
        private Vector2 moveDir;

        private void OnEnable()
        {
            speed = player.speed;
            fsm = new(Enum.GetNames(typeof(PlayerStates)).Length, Enum.GetNames(typeof(PlayerFlags)).Length);
            int idleState = fsm.AddNewState(new PlayerMoveState(ActivateOnMoveEffects, this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player));
            int attackState = fsm.AddNewState(new PlayerAttackState(ChangeFromEndAttack, ActivateOnMoveEffects,
                this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player, comboList, _attackCollider, AttackChannel, gameSettings));

            int dashState = fsm.AddNewState(new PlayerDashState(ActivateOnMoveEffects, ActivateOnDashEffects,
                ChangeFromDashEnd,
                this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player));
            int specialState = fsm.AddNewState(new PlayerSpecialAttackState(ChangeFromEndAttack,
                ActivateSpawnMovesPosition, ActivateOnMoveEffects,
                this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player, specialAttack, _attackCollider, SpecialAttackChannel, gameSettings));

            fsm.SetTranstions(idleState, PlayerFlags.Attack, attackState);
            fsm.SetTranstions(idleState, PlayerFlags.OnSpecialAttack, specialState);
            fsm.SetTranstions(attackState, PlayerFlags.EndAttack, idleState);
            fsm.SetTranstions(specialState, PlayerFlags.EndAttack, idleState);
            fsm.SetTranstions(idleState, PlayerFlags.OnDashStart, dashState);
            fsm.SetTranstions(dashState, PlayerFlags.OnDashEnd, idleState);
            AttackChannel.Subscribe(ChangeFromAttack);
            SpecialAttackChannel.Subscribe(ChangeFromSpecialAttack);
            DashChannel.Subscribe(ChangeFromDashStart);
            fsm.SetDefaultState(idleState);
        }

        private void ActivateOnDashEffects()
        {
            onDash.Invoke();
        }

        private void ChangeFromDashEnd()
        {
            onEndDash.Invoke();
            fsm.OnTriggerState(PlayerFlags.OnDashEnd);
        }

        private void ChangeFromDashStart()
        {
            fsm.OnTriggerState(PlayerFlags.OnDashStart);
        }

        private void Update()
        {
            fsm.Update(Time.deltaTime);
        }

        private void ChangeFromAttack()
        {
            fsm.OnTriggerState(PlayerFlags.Attack);
        }

        private void ChangeFromSpecialAttack()
        {
            fsm.OnTriggerState(PlayerFlags.OnSpecialAttack);
        }

        private void ChangeFromEndAttack()
        {
            fsm.OnTriggerState(PlayerFlags.EndAttack);
            // _playerAnimatorController.CrossFade("NormalStatus", 0.75f, 0, 0);
        }

        private void ActivateOnMoveEffects()
        {
            onMove.Invoke();
        }

        private void ActivateSpawnMovesPosition(Vector3 position)
        {
            var specialAttackObject = Instantiate(specialAttackVFX, position, Quaternion.identity);
            specialAttackObject.transform.position = position;
            specialAttackObject.Play(true);
            // specialAttackVFX
        }

        public void ChangeFromPause(bool value)
        {
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
            SpecialAttackChannel.Unsubscribe(ChangeFromSpecialAttack);
            DashChannel.Unsubscribe(ChangeFromDashStart);
        }
    }
}