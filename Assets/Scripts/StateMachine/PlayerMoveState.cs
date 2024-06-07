using System.Collections;
using UnityEngine;

namespace StateMachine
{
    public class PlayerMoveState : PlayerBaseState
    {
        private const float angle = -45;
        private float rotationSpeed = 10f;

        public PlayerMoveState(params object[] data) : base(data)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _playerAnimatorController.SetFloat("Blend", 0);
        }

        public override void OnTick(params object[] data)
        {
            base.OnTick(data);
        }


        public override IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                if (!isPause)
                {
                    Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                    float time = Time.deltaTime;
                    var rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                    Rotate(rotatedMoveDir);

                    _characterController.Move(rotatedMoveDir * (time * player.speed));
                    //transform.position += moveDir * (time * speed);


                    _playerAnimatorController.SetFloat("Blend", dir.magnitude);
                }

                yield return null;
            }

            _playerAnimatorController.SetFloat("Blend", 0);
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
            base.OnDestroy();
        }
    }
}