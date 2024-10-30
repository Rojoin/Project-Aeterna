using System;

namespace StateMachine
{
    public class PlayerAutomatedMoveState : PlayerMoveState
    { 
        public PlayerAutomatedMoveState(Action onMove, params object[] data) : base(onMove, data)
        {
            
        }

        protected override void Move(float deltaTime)
        {
            _characterController.Move(owner.transform.forward * (deltaTime * player.speed));
            _playerAnimatorController.SetFloat(Blend, 1);
            onMove.Invoke();
        }
    }
}