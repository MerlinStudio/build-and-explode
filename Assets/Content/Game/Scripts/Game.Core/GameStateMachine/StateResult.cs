using Configs;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.View.Panels;
using State.Result.Controllers;

namespace Game.Core.GameStateMachine
{
    public class StateResult : AbstractStateBase
    {
        public StateResult(StateResultDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }
        
        private readonly StateResultDependencies m_dependencies;

        private LevelMapController m_levelMapController;

        public override async void InitState()
        {
            var levelMapPanel = await m_dependencies.UiManager.ShowPanelAsync<LevelMapPanel>();
            m_levelMapController = new LevelMapController(
                levelMapPanel,
                m_dependencies.LevelsConfig,
                m_dependencies.LevelMapConfig);
            m_levelMapController.Init();
        }

        public override void DeinitState()
        {
            m_levelMapController.DeInit();
        }
        
        public class StateResultDependencies : StateDependencies
        {
            public IGameStateSwitcher GameStateSwitcher;
            public UiManager UiManager;
            public LevelsConfig LevelsConfig;
            public LevelMapConfig LevelMapConfig;
        }
    }
}