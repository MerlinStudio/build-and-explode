using Dev.Core.Ui.UI.Manager;
using Model.Creator.Interfaces;
using Panels;

namespace Game.Core.GameStateMachine
{
    public class StateBuild : AbstractStateBase
    {
        public StateBuild(StateBuildDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }

        private readonly StateBuildDependencies m_dependencies;
        private BuildClickerPanel m_buildClickerPanel;
        
        public override async void InitState()
        {
            m_buildClickerPanel = await m_dependencies.UiManager.ShowPanelAsync<BuildClickerPanel>();
            m_buildClickerPanel.EventButtonClicked += OnCreateBlock;
            m_dependencies.BuildCreator.Init(0);
            m_dependencies.BuildCreator.EventBuildFinished += SwitchState;
        }

        public override void DeinitState()
        {
            m_dependencies.UiManager.HidePanel<BuildClickerPanel>();
            m_buildClickerPanel.EventButtonClicked -= OnCreateBlock;
            m_dependencies.BuildCreator.DeInit();
            m_dependencies.BuildCreator.EventBuildFinished -= SwitchState;
        }

        private void OnCreateBlock()
        {
            m_dependencies.BuildCreator.CreateBlock();
        }

        private void SwitchState()
        {
            m_dependencies.BuildCreator.EventBuildFinished -= SwitchState;
            m_dependencies.GameStateSwitcher.SwitchState<StateExplosion>();
        }
        
        public class StateBuildDependencies : StateDependencies
        {
            public IGameStateSwitcher GameStateSwitcher;
            public IBuildCreator BuildCreator;
            public UiManager UiManager;
        }
    }
}