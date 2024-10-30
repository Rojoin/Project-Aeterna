using System;
using UnityEngine;
using CustomSceneSwitcher.Switcher.Core;

namespace CustomSceneSwitcher.Switcher
{
    [RequireComponent(typeof(Animator))]
    public class TransitionControl : MonoBehaviour
    {
        public event Action<SceneSwitcherEnumState> OnStateEnter;
        public event Action<SceneSwitcherEnumState> OnStateExit; 

        private Animator _animator;
        private SceneSwitcherAnimationBehaviour _sceneSwitcherAnimationBehaviour;
        
        private bool _isLocked;
        private bool _isLooping;
        private float _loopTime;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _sceneSwitcherAnimationBehaviour = _animator.GetBehaviour<SceneSwitcherAnimationBehaviour>();
            _sceneSwitcherAnimationBehaviour.OnAnimationStateEnter += OnAnimationStateEnter;
            _sceneSwitcherAnimationBehaviour.OnAnimationStateExit += OnAnimationStateExit;
        }

        private void Update()
        {
            if (!_isLooping) return;
            
            _loopTime -= Time.deltaTime;
            if (_loopTime < 0)
            {
                _isLooping = false;
                _isLocked = false;
            }
        }

        private void OnDestroy()
        {
            _sceneSwitcherAnimationBehaviour.OnAnimationStateEnter -= OnAnimationStateEnter;
            _sceneSwitcherAnimationBehaviour.OnAnimationStateExit -= OnAnimationStateExit;
        }

        public void StartTransition()
        {
            _isLocked = true;
            _sceneSwitcherAnimationBehaviour.ChangeShowState(_animator, true);
        }

        public void StopTransition()
        {
            if (_isLocked) 
                return;
            
            _sceneSwitcherAnimationBehaviour.ChangeShowState( _animator, false);
        }

        public bool IsLocked() => _isLocked;
        
        public void SetLoopTime(float loopTime)
        { 
            _loopTime = loopTime;
        }
        
        public void SetTransitionSpeed(float speed) => _animator.speed = speed;

        private void OnAnimationStateEnter(SceneSwitcherEnumState sceneSwitcherEnumState)
        {
            if(sceneSwitcherEnumState == SceneSwitcherEnumState.LoopShow)
                _isLooping = true;
            
            OnStateEnter?.Invoke(sceneSwitcherEnumState);
        }

        private void OnAnimationStateExit(SceneSwitcherEnumState sceneSwitcherEnumState)
        {
            OnStateExit?.Invoke(sceneSwitcherEnumState);
        }
    }
}
