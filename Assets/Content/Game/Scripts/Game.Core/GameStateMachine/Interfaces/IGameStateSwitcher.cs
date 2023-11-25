namespace Game.Core.GameStateMachine
{
    public interface IGameStateSwitcher
    {
        void SwitchState<T>() where T : AbstractStateBase;
    }
}