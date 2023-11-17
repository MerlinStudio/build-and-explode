namespace Game.Core.GameStateMachine
{
    public interface IGameStateSwitcher
    {
        void Init(SwitcherDependencies switcherDependencies);
        void SwitchState<T>() where T : AbstractStateBase;
    }
}