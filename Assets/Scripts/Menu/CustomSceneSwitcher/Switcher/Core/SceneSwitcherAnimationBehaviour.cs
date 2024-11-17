using System;
using UnityEngine;

namespace CustomSceneSwitcher.Switcher.Core
{
    public class SceneSwitcherAnimationBehaviour : StateMachineBehaviour
    {
        public Action<SceneSwitcherEnumState> OnAnimationStateEnter;
        public Action<SceneSwitcherEnumState> OnAnimationStateExit;

        private readonly int _transitionBoolName = Animator.StringToHash("Show"); 
        
        private readonly int _showTag = Animator.StringToHash("SHOW");
        private readonly int _loopShowTag = Animator.StringToHash("LOOP_SHOW");
        private readonly int _hideTag = Animator.StringToHash("HIDE");

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.tagHash == _showTag)
            {
                OnAnimationStateEnter?.Invoke(SceneSwitcherEnumState.Showing);
            }
            else if (stateInfo.tagHash == _loopShowTag)
            {
                OnAnimationStateEnter?.Invoke(SceneSwitcherEnumState.LoopShow);
            }
            else if (stateInfo.tagHash == _hideTag)
            {
                OnAnimationStateEnter?.Invoke(SceneSwitcherEnumState.Hiding);
            }
        }

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.tagHash == _showTag)
            {
                OnAnimationStateExit?.Invoke(SceneSwitcherEnumState.Showing);
            }
            else if (stateInfo.tagHash == _loopShowTag)
            {
                OnAnimationStateExit?.Invoke(SceneSwitcherEnumState.LoopShow);
            }
            else if (stateInfo.tagHash == _hideTag)
            {
                OnAnimationStateExit?.Invoke(SceneSwitcherEnumState.Hiding);
            }
        }

        public void ChangeShowState(Animator anim, bool show) => anim.SetBool(_transitionBoolName, show);
    }
}
