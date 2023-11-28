using Zenject;

namespace Game.Core.GameStateMachine.Interfaces
{
    public abstract class AbstractStateBase : ITickable
    {
        public abstract void InitState();
        public abstract void DeinitState();
        public virtual void Tick()
        {
        }
    }
}