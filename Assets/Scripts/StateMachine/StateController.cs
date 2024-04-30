using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StateMachine
{
    public class StateController<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

        protected BaseState<EState> _currentState;
        private void Start()
        {
            EnterNewState();
        }

        private void EnterNewState()
        {
            _currentState.EnterState();
            foreach (Action<EState> stateEvent in _currentState.changeStateEvents)
            {
                var transitionToState = stateEvent;
                transitionToState += TransitionToState;
            }
        }

        private void Update()
        {
            _currentState.UpdateState(Time.deltaTime);
        }

        private void TransitionToState(EState stateKey)
        {
            _currentState.ExitState();
            _currentState = States[stateKey];
            EnterNewState();
        }
        private void OnTriggerEnter(Collider other)
        {
            _currentState.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            _currentState.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _currentState.OnTriggerExit(other);
        }
    }
}