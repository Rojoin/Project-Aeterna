using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    //Todo: Change vairables of FSM to be more generic
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
            currentState.OnEnter();
        }

        public void SetTranstions(int currentStateKey, PlayerFlags flag, int stateToChange)
        {
            transitions[currentStateKey, (int)flag] = stateToChange;
        }

        public void OnTriggerState(PlayerFlags flag)
        {
            int posTransition = transitions[states.IndexOf(currentState), (int)flag];
            if (posTransition != -1)
            {
                currentState.OnExit();
                currentState = states[posTransition];
                currentState.OnEnter();
                Debug.Log(states[posTransition]);
            }
        }

        public int AddNewState(BaseStateFSM newBaseState)
        {
            states.Add(newBaseState);
            return states.IndexOf(newBaseState);
        }

        public void Update(params object[] data)
        {
            currentState.OnTick(data);
        }

        public void OnDestroy()
        {
            foreach (BaseStateFSM baseState in states)
            {
                baseState.OnDestroy();
            }
        }

    
    }
}