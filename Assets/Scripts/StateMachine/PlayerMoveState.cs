using System;
using System.Collections;
using UnityEngine;

namespace StateMachine
{
    public class PlayerMoveState : PlayerBaseState
    {
        private const float angle = -45;
        private float rotationSpeed = 10f;
        private static readonly int Blend = Animator.StringToHash("Blend");

        public PlayerMoveState(Action onMove, params object[] data) : base(onMove, data)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _playerAnimatorController.SetFloat(Blend, 0);
        }

        public override void OnTick(params object[] data)
        {
            base.OnTick(data);
        }

        protected override void Move(float deltaTime)
        {
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                var rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Rotate(rotatedMoveDir);
                _characterController.Move(rotatedMoveDir * (deltaTime * player.speed));
                _playerAnimatorController.SetFloat(Blend, inputDirection.magnitude);
                onMove.Invoke();
            }
            else
            {
                _playerAnimatorController.SetFloat(Blend, 0);
            }
        }


        public override void Rotate(Vector3 newDirection)
        {
            owner.transform.forward =
                Vector3.Slerp(owner.transform.forward, newDirection, Time.deltaTime * rotationSpeed);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}