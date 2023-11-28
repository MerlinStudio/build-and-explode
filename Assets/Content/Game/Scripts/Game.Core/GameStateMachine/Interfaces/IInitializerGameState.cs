namespace Game.Core.GameStateMachine.Interfaces
{
    public interface IInitializerGameState
    {
        void Init(params AbstractStateBase[] allStates);
        void DeInit();
    }
}