﻿using System;
using UnityEngine;


namespace StateMachine
{
    public class PlayerDashState : PlayerBaseState
    {
        private float timer;
        private Action OnDashEnd;
        private Action OnDashStart;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private AnimationCurve dashCurve;
        private static readonly int Dash1 = Animator.StringToHash("Dash");
        readonly LayerMask wallLayer = LayerMask.GetMask("Wall");

        public PlayerDashState(Action onMove, Action OnDashStart, Action OnDashEnd, params object[] data) : base(onMove,
            data)
        {
            this.OnDashEnd += OnDashEnd;
            this.OnDashStart += OnDashStart;
            dashCurve = player.dashCurve;
        }

        public override void OnEnter()
        {
            startPosition = _characterController.transform.position;
            _characterController.detectCollisions = false;
            owner.layer = LayerMask.NameToLayer("PlayerDashing");
            timer = 0.0f;
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
            }
            else
            {
                rotatedMoveDir = new Vector3(_characterController.transform.forward.x, 0,
                    _characterController.transform.forward.z);
                ;
            }

            Rotate(rotatedMoveDir);
            endPosition = CalculateMaxDashDistance();
            _playerAnimatorController.SetTrigger(Dash1);
            OnDashStart.Invoke();
        }

        private Vector3 CalculateMaxDashDistance()
        {
            float maxPlayerDistance = (player.dashSpeed * player.dashTimer);
            if (Physics.Raycast(startPosition, rotatedMoveDir, out RaycastHit hit, maxPlayerDistance, wallLayer))
            {
                Debug.Log($"The final position is{startPosition + rotatedMoveDir * hit.distance}");
                return startPosition + rotatedMoveDir * (hit.distance - 0.3f);
            }

            return startPosition + rotatedMoveDir * maxPlayerDistance;
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
                float interpolate = dashCurve.Evaluate(timer / player.dashTimer);

                Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, interpolate);
                _characterController.Move(currentPosition - _characterController.transform.position);
            }
            else
            {
                OnDashEnd.Invoke();
            }
        }

        protected override void Move(float deltaTime)
        {
            //_characterController.Move(rotatedMoveDir * (deltaTime * player.dashSpeed));
            //   onMove.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
            timer = 0.0f;
            _characterController.detectCollisions = true;
            owner.layer = LayerMask.NameToLayer("Player");
        }
    }
}