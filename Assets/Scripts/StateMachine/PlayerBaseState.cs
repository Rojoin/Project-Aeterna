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
        protected Coroutine movement;
        protected Vector3 rotatedMoveDir;
        static public Vector2 inputDirection;
        static public bool isPause = false;
        protected const float angle = -45;

        public PlayerBaseState(params object[] data) : base(data)
        {
            _playerAnimatorController = data[1] as Animator;
            _characterController = data[2] as CharacterController;
            OnMoveChannel = data[3] as Vector2ChannelSO;
            player = data[4] as EntitySO;
        }

        protected virtual void Move(float deltaTime)
        {
            if (isPause)
                return;
            
            if (inputDirection != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(inputDirection.x, 0, inputDirection.y);
                rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Rotate(rotatedMoveDir);

                _characterController.Move(rotatedMoveDir * (deltaTime * player.speed));
                //transform.position += moveDir * (time * speed);
                _playerAnimatorController.SetFloat("Blend", inputDirection.magnitude);
            }
            else
            {
                rotatedMoveDir = Vector2.zero;
                _playerAnimatorController.SetFloat("Blend", 0);
            }
        }

        public override void OnEnter()
        {
            OnMoveChannel.Subscribe(ChangeInputDirection);
        }

        private void ChangeInputDirection(Vector2 obj)
        {
            inputDirection = obj;
        }

        public override void OnTick(params object[] data)
        {
            float deltaTime = (float)data[0];
            Move(deltaTime);
        }

        public override void OnExit()
        {
            OnMoveChannel.Unsubscribe(ChangeInputDirection);
        }

        public override void OnDestroy()
        {
            owner.GetComponent<MonoBehaviour>().StopAllCoroutines();
            OnMoveChannel.Unsubscribe(ChangeInputDirection);
        }

        public virtual void Rotate(Vector3 newDirection)
        {
            owner.transform.forward = Vector3.Slerp(owner.transform.forward, newDirection, 1);
        }

        public virtual IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                if (!isPause)
                {
                    Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                    float time = Time.deltaTime;
                    rotatedMoveDir = Quaternion.AngleAxis(-45, Vector3.up) * moveDir;
                    Rotate(rotatedMoveDir);

                    _characterController.Move(rotatedMoveDir * (time * player.speed));
                    //transform.position += moveDir * (time * speed);


                    _playerAnimatorController.SetFloat("Blend", dir.magnitude);
                }

                yield return null;
            }

            rotatedMoveDir = Vector2.zero;
            _playerAnimatorController.SetFloat("Blend", 0);
        }

        protected Vector3 GetRotatedMoveDir() => rotatedMoveDir;
    }
}