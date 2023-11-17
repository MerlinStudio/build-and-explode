namespace Game.Core.GameStateMachine
{
    public abstract class AbstractStateBase
    {
        protected AbstractStateBase(StateDependencies dependencies)
        {
        }

        public abstract void InitState();
        public abstract void DeinitState();
        
        public abstract class StateDependencies
        {
        }
    }
}