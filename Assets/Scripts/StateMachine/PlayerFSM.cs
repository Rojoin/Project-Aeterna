using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
        OnAutomatingStart,
        ForceIdle,
        Pause
    }

    public enum PlayerStates
    {
        Move,
        Attack,
        SpecialAttack,
        AutomatedMovement,
        Dash
    }

    public class PlayerFSM : MonoBehaviour
    {
        [Header("Channels")] [SerializeField] protected Vector2ChannelSO OnMoveChannel;
        [SerializeField] protected VoidChannelSO AttackChannel;
        [SerializeField] protected VoidChannelSO SpecialAttackChannel;
        [SerializeField] protected VoidChannelSO DashChannel;
        [SerializeField] protected VoidChannelSO AutomatedMovementChannel;
        [SerializeField] protected BoolChannelSO OnDeathChannel;

        [Header("References")] [SerializeField]
        protected GameSettings gameSettings;

        [SerializeField] protected PlayerEntitySO player;
        [SerializeField] protected Animator _playerAnimatorController;
        [SerializeField] protected CharacterController _characterController;
        [SerializeField] private List<AttackSO> comboList;
        [SerializeField] private AttackSO specialAttack;
        [SerializeField] private AttackCollision _attackCollider;
        [SerializeField] private ParticleSystem specialAttackVFX;

        [Header("Particles")] [SerializeField] private ParticleSystem vfxSpecialAura;

        [Header("Unity Events")] [SerializeField]
        private UnityEvent onMove;

        [SerializeField] private UnityEvent onDash;
        [SerializeField] private UnityEvent onEndDash;
        [SerializeField] private UnityEvent OnSpecialAttack;
        [SerializeField] private UnityEvent<float> OnSpecialAttackTimerUpdate;

        [Header("Raycast Settings")] [SerializeField]
        protected float raycastDistance = 10.0f;

        [SerializeField] protected Vector3 raycastOffset;
        private bool _isGettingInteractable = false;
        private IInteractable interactable = null;
        protected float speed;
        private FSM fsm;
        private Vector2 moveDir;
        private float specialAttackTimer = 0;

        public VoidChannelSO OnInteractChannel;
        public VoidChannelSO OnAlternativeInteractChannel;

        private void OnEnable()
        {
            speed = player.speed;
            InitFSM();

            AttackChannel.Subscribe(ChangeFromAttack);
            SpecialAttackChannel.Subscribe(ChangeFromSpecialAttack);
            DashChannel.Subscribe(ChangeFromDashStart);
            AutomatedMovementChannel.Subscribe(ChangeToAutomatedMovement);
            OnDeathChannel.Subscribe(OnDeath);
            OnInteractChannel.Subscribe(SetInteract);
            OnAlternativeInteractChannel.Subscribe(SetAlternativeInteract);
            specialAttackTimer = specialAttack.timeUntilComboEnds;
        }

        private void OnDeath(bool value)
        {
            InputControls.InputController.IsGamePaused = true;
        }

        private void InitFSM()
        {
            fsm = new(Enum.GetNames(typeof(PlayerStates)).Length, Enum.GetNames(typeof(PlayerFlags)).Length);
            int idleState = fsm.AddNewState(new PlayerMoveState(ActivateOnMoveEffects, this.gameObject,
                _playerAnimatorController,
                _characterController, OnMoveChannel, player));
            int automatedMoveState = fsm.AddNewState(new PlayerAutomatedMoveState(ActivateOnMoveEffects,
                this.gameObject,
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
                _characterController, OnMoveChannel, player, specialAttack, _attackCollider, SpecialAttackChannel,
                gameSettings));

            fsm.SetTranstions(idleState, PlayerFlags.Attack, attackState);
            fsm.SetTranstions(idleState, PlayerFlags.OnSpecialAttack, specialState);
            fsm.SetTranstions(attackState, PlayerFlags.EndAttack, idleState);
            fsm.SetTranstions(specialState, PlayerFlags.EndAttack, idleState);
            fsm.SetTranstions(idleState, PlayerFlags.OnDashStart, dashState);
            fsm.SetTranstions(idleState, PlayerFlags.OnAutomatingStart, automatedMoveState);
            fsm.SetTranstions(attackState, PlayerFlags.OnAutomatingStart, automatedMoveState);
            fsm.SetTranstions(specialState, PlayerFlags.OnAutomatingStart, automatedMoveState);
            fsm.SetTranstions(dashState, PlayerFlags.OnAutomatingStart, automatedMoveState);
            fsm.SetTranstions(automatedMoveState, PlayerFlags.ForceIdle, idleState);
            fsm.SetTranstions(dashState, PlayerFlags.ForceIdle, idleState);
            fsm.SetTranstions(attackState, PlayerFlags.ForceIdle, idleState);
            fsm.SetTranstions(specialState, PlayerFlags.ForceIdle, idleState);
            fsm.SetTranstions(dashState, PlayerFlags.OnDashEnd, idleState);
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

        public void ForceToIdle()
        {
            fsm.OnTriggerState(PlayerFlags.ForceIdle);
        }

        private void ChangeToAutomatedMovement()
        {
            onEndDash.Invoke();
            fsm.OnTriggerState(PlayerFlags.OnAutomatingStart);
        }

        private void ChangeFromDashStart()
        {
            fsm.OnTriggerState(PlayerFlags.OnDashStart);
        }

        private void Update()
        {
            _isGettingInteractable =
                IsGettingInteractable();

            fsm.Update(Time.deltaTime);
            UpdateSpecialAttackTimer();
        }

        private bool IsGettingInteractable()
        {
            bool a = Physics.Raycast(transform.position + raycastOffset, transform.forward, out RaycastHit hit,
                raycastDistance);
            if (!a)
            {
                if (interactable != null)
                {
                    interactable?.ToggleDialogBox(false);
                    interactable = null;
                }

                return false;
            }

            var interactable1 = hit.collider.GetComponent<IInteractable>();
            if (interactable1 != null)
            {
                if (interactable1 == interactable)
                {
                    return true;
                }

                interactable = interactable1;
                interactable?.ToggleDialogBox(true);
                return true;
            }
            else if (interactable != null)
            {
                interactable?.ToggleDialogBox(false);
                interactable = null;
            }

            return false;
        }

        private void UpdateSpecialAttackTimer()
        {
            float specialAttackTimeUntilComboEnds = specialAttackTimer / specialAttack.timeUntilComboEnds;
            if (specialAttackTimer < specialAttack.timeUntilComboEnds)
            {
                specialAttackTimer += Time.deltaTime;
                specialAttackTimeUntilComboEnds = specialAttackTimer / specialAttack.timeUntilComboEnds;
                if (specialAttackTimer >= specialAttack.timeUntilComboEnds)
                {
                    Debug.Log($"VFXAURAPLAY");
                    vfxSpecialAura?.Play();
                }
            }

            OnSpecialAttackTimerUpdate.Invoke(specialAttackTimeUntilComboEnds);
        }


        private void ChangeFromAttack()
        {
            fsm.OnTriggerState(PlayerFlags.Attack);
        }

        public void ResetSpecialAttack()
        {
            specialAttackTimer = specialAttack.timeUntilComboEnds;
        }

        private void ChangeFromSpecialAttack()
        {
            if (specialAttackTimer >= specialAttack.timeUntilComboEnds)
            {
                fsm.OnTriggerState(PlayerFlags.OnSpecialAttack);
                specialAttackTimer = 0;
                OnSpecialAttackTimerUpdate.Invoke(0);
            }
            else
            {
                //TODO: Add BadSound
            }
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
            OnSpecialAttack?.Invoke();
        }

        public void ChangeFromPause(bool value)
        {
            PlayerBaseState.inputDirection = Vector2.zero;
            _playerAnimatorController.speed = value ? 0 : 1;
            ChangeFromEndAttack();
        }

        #region Interact

        public void SetInteract() => SetInteractable(0);
        public void SetAlternativeInteract() => SetInteractable(1);

        private void SetInteractable(int value)
        {
            if (_isGettingInteractable)
            {
                interactable.Interact(value);
            }
        }

        #endregion

        private void MoveDirection(Vector2 newMoveDir)
        {
            moveDir = newMoveDir;
        }

        private void OnDisable()
        {
            fsm.OnDestroy();
            AttackChannel.Unsubscribe(ChangeFromAttack);
            SpecialAttackChannel.Unsubscribe(ChangeFromSpecialAttack);
            AutomatedMovementChannel.Unsubscribe(ChangeToAutomatedMovement);
            DashChannel.Unsubscribe(ChangeFromDashStart);
            OnInteractChannel.Unsubscribe(SetInteract);
            OnAlternativeInteractChannel.Unsubscribe(SetAlternativeInteract);
            OnDeathChannel.Unsubscribe(OnDeath);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position + raycastOffset,
                transform.position + raycastOffset + transform.forward * raycastDistance);
        }
    }
}