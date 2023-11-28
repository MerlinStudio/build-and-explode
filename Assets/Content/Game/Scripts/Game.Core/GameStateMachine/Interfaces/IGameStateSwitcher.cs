namespace Game.Core.GameStateMachine.Interfaces
{
    public interface IGameStateSwitcher
    {
        void SwitchState<T>() where T : AbstractStateBase;
    }
}