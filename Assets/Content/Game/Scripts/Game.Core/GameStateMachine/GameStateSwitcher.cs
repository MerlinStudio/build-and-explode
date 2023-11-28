using System.Collections.Generic;
using Game.Core.GameStateMachine.Interfaces;
using Zenject;

namespace Game.Core.GameStateMachine
{
    public class GameStateSwitcher : IInitializerGameState, IGameStateSwitcher, ITickable
    {
        private ITickable m_currentTick;
        private AbstractStateBase m_currentState;
        private List<AbstractStateBase> m_allStates;

        public void Init(params AbstractStateBase[] allStates)
        {
            m_allStates = new List<AbstractStateBase>(allStates);
        }
        
        public void DeInit()
        {
            m_allStates.Clear();
        }
        
        public void SwitchState<T>() where T : AbstractStateBase
        {
            var state = m_allStates.Find(s => s is T);
            m_currentState?.DeinitState();
            m_currentState = state;
            m_currentState.InitState();
            m_currentTick = m_currentState;
        }

        public void Tick()
        {
            m_currentTick?.Tick();
        }
    }
}