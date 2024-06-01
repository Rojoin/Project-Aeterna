using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class Player : MonoBehaviour
    {
        private FSM playerFsm;
        private Rigidbody a;

        private void Start()
        {
            playerFsm = new(2, 2);
            int idleState = playerFsm.AddNewState(new PlayerFSM(this.gameObject, a));

            playerFsm.SetTranstions(0, 0, 1);
            playerFsm.SetTranstions(1, 1, 0);
        }

        private void Update()
        {
            playerFsm.Update();
        }
    }

    public enum FSMFlags
    {
        Move,
        Attack
    }

    public sealed class FSM
    {
        protected List<BaseStateFSM> states;
        protected BaseStateFSM currentState;
        protected int[,] transitions;

        private Animator _playerAnimatorController;
        private CharacterController _characterController;
        private Vector2ChannelSO OnMoveChannel;

        public FSM(int posibleStates, int flagsQty)
        {
            transitions = new int[posibleStates, flagsQty];
            states = new List<BaseStateFSM>();
            for (int i = 0; i < posibleStates; i++)
            {
                for (int j = 0; j < flagsQty; j++)
                {
                    transitions[i, j] = -1;
                }
            }
        }

        public void SetDefaultState(int id)
        {
            currentState = states[id];
        }

        public void SetTranstions(int currentStateKey, int flag, int stateToChange)
        {
            transitions[currentStateKey, flag] = stateToChange;
        }

        public void OnTriggerState(int flag)
        {
            int posTransition = transitions[states.IndexOf(currentState), flag];
            if (posTransition != -1)
            {
                currentState.OnExit();
                currentState = states[posTransition];
                currentState.OnEnter();
            }
        }

        public int AddNewState(BaseStateFSM newBaseState)
        {
            states.Add(newBaseState);
            return states.IndexOf(newBaseState);
        }

        public void Update()
        {
            currentState.OnTick();
        }
    }

    public abstract class BaseStateFSM
    {
        protected GameObject owner;

        public BaseStateFSM(params object[] data)
        {
            owner = data[0] as GameObject;
        }

        public abstract void OnEnter();
        public abstract void OnTick(params object[] data);
        public abstract void OnExit();
    }

    public class PlayerFSM : BaseStateFSM
    {
        protected Animator _playerAnimatorController;
        protected CharacterController _characterController;
        protected Vector2ChannelSO OnMoveChannel;
        protected VoidChannelSO AttackChannel;
        protected EntitySO player;
        protected float speed;
        protected Coroutine movement;

        public PlayerFSM(params object[] data) : base(data)
        {
            _playerAnimatorController = data[1] as Animator;
            _characterController = data[2] as CharacterController;
            OnMoveChannel = data[3] as Vector2ChannelSO;
            speed = (float)data[4];
        }

        public override void OnEnter()
        {
            throw new NotImplementedException();
        }

        public override void OnTick(params object[] data)
        {
            throw new NotImplementedException();
        }

        public override void OnExit()
        {
            throw new NotImplementedException();
        }

        public virtual void Rotate(Vector3 newDirection)
        {
            owner.transform.forward = Vector3.Slerp(owner.transform.forward, newDirection, 1);
        }
    }

    public class PlayerMoveState : PlayerFSM
    {
        private const float angle = -45;
        private float rotationSpeed = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
            OnMoveChannel.Subscribe(Move);
        }

        private void Move(Vector2 dir)
        {
            if (movement != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(movement);
            }

            movement = owner.GetComponent<MonoBehaviour>().StartCoroutine(Movement(dir));
        }

        private IEnumerator Movement(Vector2 dir)
        {
            while (dir != Vector2.zero)
            {
                Vector3 moveDir = new Vector3(dir.x, 0, dir.y);
                float time = Time.deltaTime;
                var rotatedMoveDir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Rotate(rotatedMoveDir);

                _characterController.Move(rotatedMoveDir * (time * player.speed));
                //transform.position += moveDir * (time * speed);


                _playerAnimatorController.SetFloat("Blend", dir.magnitude);

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
            if (movement != null)
            {
                owner.GetComponent<MonoBehaviour>().StopCoroutine(movement);
            }
        }
    }
}