using State.Explosion.Interfaces;

namespace Game.Core.GameStateMachine
{
    public abstract class AbstractStateBase : IGameTick
    {
        protected AbstractStateBase(StateDependencies dependencies)
        {
        }

        public abstract void InitState();
        public abstract void DeinitState();
        
        public abstract class StateDependencies
        {
        }

        public virtual void Tick()
        {
        }
    }
}