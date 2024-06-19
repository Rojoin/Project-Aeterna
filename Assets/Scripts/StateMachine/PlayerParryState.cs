using System;

namespace StateMachine
{
    public class PlayerParryState : PlayerBaseState
    {
        public PlayerParryState(Action onMove, params object[] data) : base(onMove, data)
        {
        }
    }
}