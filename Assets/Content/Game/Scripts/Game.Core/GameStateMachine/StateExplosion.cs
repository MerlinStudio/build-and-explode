using Data.Explosion.Configs;
using Dev.Core.Ui.UI.Manager;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using Panels;

namespace Game.Core.GameStateMachine
{
    public class StateExplosion : AbstractStateBase
    {
        public StateExplosion(StateExplosionDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }

        private readonly StateExplosionDependencies m_dependencies;
        private PreparationExplosionPanel m_preparationExplosionPanel;
        
        public override async void InitState()
        {
            m_dependencies.UiManager.HidePanel<BuildClickerPanel>();
            m_preparationExplosionPanel = await m_dependencies.UiManager.ShowPanelAsync<PreparationExplosionPanel>();
            m_preparationExplosionPanel.EventExplosion += OnExplosion;
            m_preparationExplosionPanel.EventAddBomb += OnAddBomb;
            m_preparationExplosionPanel.EventBuildingLayerChange += OnChangeBuildingLayer;
            m_dependencies.ExplosionManager.Init(m_dependencies.BuildProvider, m_dependencies.BlockCreator, m_dependencies.EnvironmentInfoConfig);
        }

        public override void DeinitState()
        {
            m_preparationExplosionPanel.EventExplosion -= OnExplosion;
            m_preparationExplosionPanel.EventAddBomb -= OnAddBomb;
            m_preparationExplosionPanel.EventBuildingLayerChange -= OnChangeBuildingLayer;
            m_dependencies.UiManager.HidePanel<PreparationExplosionPanel>();
            m_dependencies.ExplosionManager.DeInit();
        }

        private void OnExplosion()
        {
            m_preparationExplosionPanel.EventExplosion -= OnExplosion;
            m_dependencies.UiManager.HidePanel<PreparationExplosionPanel>();
            m_dependencies.ExplosionManager.Explosion();
        }
        
        private void OnAddBomb(int radius, int force, float delay)
        {
            m_dependencies.ExplosionManager.AddBombInfo(radius, force, delay);
        }

        private void OnChangeBuildingLayer(float value)
        {
            m_dependencies.ExplosionManager.ChangeBuildingLayer(value);
        }
        
        public class StateExplosionDependencies : StateDependencies
        {
            public IGameStateSwitcher GameStateSwitcher;
            public IBuildProvider BuildProvider;
            public IBlockCreator BlockCreator;
            public IExplosionManager ExplosionManager;
            public EnvironmentInfoConfig EnvironmentInfoConfig;
            public UiManager UiManager;
        }
    }
}