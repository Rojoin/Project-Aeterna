using UnityEngine;

namespace StateMachine
{
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
        public abstract void OnDestroy();
    }
}