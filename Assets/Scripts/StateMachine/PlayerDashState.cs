using System;
using UnityEngine;


namespace StateMachine
{
    public class PlayerDashState : PlayerBaseState
    {
        private float timer;
        private Action OnDashEnd;

        public PlayerDashState(Action onMove, Action OnDashEnd, params object[] data) : base(onMove, data)
        {
            this.OnDashEnd += OnDashEnd;
        }

        public override void OnEnter()
        {
            timer = 0.0f;
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Rotate(rotatedMoveDir);
            }
            else
            {
                // Vector3 moveDir = new Vector3(0.7f, 0, 0.7f);
                // rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                // Rotate(rotatedMoveDir);
            }

            // {
            //     _characterController.transform.forward
            // }
        }

        public override void OnTick(params object[] data)
        {
            float deltaTime = (float)data[0];
            if (timer < player.dashTimer)
            {
                timer += deltaTime;
                Dash(deltaTime);
            }
            else
            {
                OnDashEnd.Invoke();
            }
        }

        protected void Dash(float deltaTime)
        {
            if (rotatedMoveDir != Vector3.zero)
            {
                _characterController.Move(rotatedMoveDir * (deltaTime * player.dashSpeed));
            }
            else
            {
                OnDashEnd.Invoke();
            }
            //   onMove.Invoke();
        }

        protected override void Move(float deltaTime)
        {
            //_characterController.Move(rotatedMoveDir * (deltaTime * player.dashSpeed));
            //   onMove.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
            inputDirection = Vector2.zero;
            timer = 0.0f;
        }
    }
}