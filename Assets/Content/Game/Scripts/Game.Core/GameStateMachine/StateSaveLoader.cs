using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using Dev.Core.Ui.UI.Manager;
using Game.View.Panels;
using State.Creator.Interfaces;
using State.SavaLoader.Interfaces;

namespace Game.Core.GameStateMachine
{
    public class StateSaveLoader : AbstractStateBase
    {
        public StateSaveLoader(StateSaveLoaderDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }
        
        private readonly StateSaveLoaderDependencies m_dependencies;

        public override async void InitState()
        {
            var task1 = m_dependencies.UiManager.ShowPanelAsync<SaveLoaderPanel>();
            var task2 =  m_dependencies.SaveConstructionController.Construction();
            await UniTask.WhenAll(task1, task2);
            if (m_dependencies.SaveConstructionController.CheckConstructionProgress())
            {
                m_dependencies.GameStateSwitcher.SwitchState<StateExplosion>();
                return;
            }
            m_dependencies.UiManager.HidePanel<SaveLoaderPanel>();
            m_dependencies.GameStateSwitcher.SwitchState<StateBuild>();
        }

        public override void DeinitState()
        {
        }
        
        public class StateSaveLoaderDependencies : StateDependencies
        {
            public UiManager UiManager;
            public IGameStateSwitcher GameStateSwitcher;
            public ISaveConstructionController SaveConstructionController;
        }
    }
}