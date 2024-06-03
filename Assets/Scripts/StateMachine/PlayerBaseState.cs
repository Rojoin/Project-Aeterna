using System.Collections;
using UnityEngine;

namespace StateMachine
{
    public class PlayerBaseState : BaseStateFSM
    {
        protected Animator _playerAnimatorController;
        protected CharacterController _characterController;
        protected Vector2ChannelSO OnMoveChannel;
        protected VoidChannelSO AttackChannel;
        protected EntitySO player;
        protected float speed;
        protected Coroutine movement;
        protected Vector3 rotatedMoveDir;
        static protected Vector2 inputDirection;

        public PlayerBaseState(params object[] data) : base(data)
        {
            _playerAnimatorController = data[1] as Animator;
            _characterController = data[2] as CharacterController;
            OnMoveChannel = data[3] as Vector2ChannelSO;
            speed = (float)data[4];
            player = data[5] as EntitySO;
        }

        protected void Move(Vector2 dir)
        {
            inputDirection = dir;
            if (movement != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(movement);
            }

            movement = owner.GetComponent<MonoBehaviour>().StartCoroutine(Movement(dir));
        }

        public override void OnEnter()
        {
            OnMoveChannel.Subscribe(Move);
            if (inputDirection != Vector2.zero)
            {
                Move(inputDirection);
            }
        }

        public override void OnTick(params object[] data)
        {
        }

        public override void OnExit()
        {
            OnMoveChannel.Unsubscribe(Move);
            if (movement != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(movement);
            }
        }

        public override void OnDestroy()
        {
        }

        public virtual void Rotate(Vector3 newDirection)
        {
            owner.transform.forward = Vector3.Slerp(owner.transform.forward, newDirection, 1);
        }

        public virtual IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                float time = Time.deltaTime;
                rotatedMoveDir = Quaternion.AngleAxis(-45, Vector3.up) * moveDir;
                Rotate(rotatedMoveDir);

                _characterController.Move(rotatedMoveDir * (time * player.speed));
                //transform.position += moveDir * (time * speed);


                _playerAnimatorController.SetFloat("Blend", dir.magnitude);

                yield return null;
            }

            rotatedMoveDir = Vector2.zero;
            _playerAnimatorController.SetFloat("Blend", 0);
        }

        protected Vector3 GetRotatedMoveDir() => rotatedMoveDir;
    }
}