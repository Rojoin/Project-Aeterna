using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum
{
    public Action<EState>[] changeStateEvents;
    protected EState nextState;
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState(float deltaTime);
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
 public enum Move
   {
       a = 1
   }
public class MoveState : BaseState<Move>
{
    public override void EnterState()
    {
      
    }

    public override void ExitState()
    {
   
    }

    public override void UpdateState(float deltaTime)
    {

    }

    public override Move GetNextState()
    {
        throw new NotImplementedException();
    }
    public override void OnTriggerEnter(Collider other)
    {
    }
    public override void OnTriggerStay(Collider other)
    {
    }
    public override void OnTriggerExit(Collider other)
    {
    }
}
