using Data.Explosion.Configs;
using Dev.Core.Ui.UI.Manager;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using Panels;
using UnityEngine;

namespace Game.Core.GameStateMachine
{
    public class StateExplosion : AbstractStateBase
    {
        public StateExplosion(StateExplosionDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }

        private readonly StateExplosionDependencies m_dependencies;
        
        public override async void InitState()
        {
            m_dependencies.UiManager.HidePanel<BuildClickerPanel>();
            var preparationExplosionPanel = await m_dependencies.UiManager.ShowPanelAsync<PreparationExplosionPanel>();
            preparationExplosionPanel.EventExplosion += OnExplosion;
            preparationExplosionPanel.EventAddBomb += OnAddBomb;
            m_dependencies.ExplosionManager.Init(m_dependencies.BuildProvider, m_dependencies.BlockCreator, m_dependencies.EnvironmentInfoConfig);
        }

        public override void DeinitState()
        {
            m_dependencies.ExplosionManager.DeInit();
        }

        private void OnExplosion()
        {
            m_dependencies.ExplosionManager.Explosion();
        }
        
        private void OnAddBomb(Transform blockTransform, int radius, int force, float delay)
        {
            m_dependencies.ExplosionManager.AddBomb(blockTransform, radius, force, delay);
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