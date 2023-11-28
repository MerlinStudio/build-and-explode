using Cysharp.Threading.Tasks;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
using Game.View.Panels;
using State.SavaLoader.Interfaces;

namespace Game.Core.GameStateMachine.States
{
    public class StateSaveLoader : AbstractStateBase
    {
        public StateSaveLoader(
            UiManager uiManager,
            IGameStateSwitcher gameStateSwitcher,
            ISaveConstructionController saveConstructionController)
        {
            m_uiManager = uiManager;
            m_gameStateSwitcher = gameStateSwitcher;
            m_saveConstructionController = saveConstructionController;
        }
        
        private readonly UiManager m_uiManager;
        private readonly IGameStateSwitcher m_gameStateSwitcher;
        private readonly ISaveConstructionController m_saveConstructionController;
        
        public override async void InitState()
        {
            var task1 = m_uiManager.ShowPanelAsync<SaveLoaderPanel>();
            var task2 =  m_saveConstructionController.Construction();
            await UniTask.WhenAll(task1, task2);
            if (m_saveConstructionController.CheckConstructionProgress())
            {
                m_gameStateSwitcher.SwitchState<StateExplosion>();
                return;
            }
            m_gameStateSwitcher.SwitchState<StateBuild>();
        }

        public override void DeinitState()
        {
            m_uiManager.HidePanel<SaveLoaderPanel>();
        }
    }
}