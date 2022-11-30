﻿
namespace DataStructures.StateLogic
{
    public abstract class BaseState : IState
    {
        public StateData Data { get; }

        protected BaseState(StateData data)
        {
            Data = data;
        }

        public virtual void Enter()
        {
            Data.OnEnterEvent.Raise(this);
            Data.IsStateActive.SetTrue();
        }

        public virtual void Execute()
        {
            
        }

        public virtual void Exit()
        {
            Data.IsStateActive.SetFalse();
            Data.OnExitEvent.Raise(this);
        }
    }
}