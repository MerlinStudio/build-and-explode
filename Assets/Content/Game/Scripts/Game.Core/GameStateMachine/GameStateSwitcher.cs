using System.Collections.Generic;
using Data.Explosion.Configs;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.Data.Models;
using Model.Creator.Controllers;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;

namespace Game.Core.GameStateMachine
{
    public class GameStateSwitcher : IGameStateSwitcher
    {
        private List<AbstractStateBase> m_allStates;
        private AbstractStateBase m_currentState;

        public void Init(SwitcherDependencies dependencies)
        {
            var buildCreator = new BuildCreator(dependencies.LevelSettings.BuildDataConfig, dependencies.BlockCreator);
            var stateBuildDependencies = new StateBuild.StateBuildDependencies
            {
                GameStateSwitcher = this,
                BuildCreator = buildCreator,
                UiManager = dependencies.UiManager
            };
            var stateExplosionDependencies = new StateExplosion.StateExplosionDependencies
            {
                GameStateSwitcher = this,
                BuildProvider = buildCreator,
                BlockCreator = dependencies.BlockCreator,
                ExplosionManager = dependencies.ExplosionManager,
                EnvironmentInfoConfig = dependencies.EnvironmentInfoConfig,
                UiManager = dependencies.UiManager
            };
            m_allStates = new List<AbstractStateBase>
            {
                new StateBuild(stateBuildDependencies),
                new StateExplosion(stateExplosionDependencies)
            };
        }
        
        public void SwitchState<T>() where T : AbstractStateBase
        {
            var state = m_allStates.Find(s => s is T);
            m_currentState?.DeinitState();
            m_currentState = state;
            m_currentState.InitState();
        }
    }

    public class SwitcherDependencies
    {
        public UiManager UiManager;
        public GameDataModel GameDataModel;
        public LevelSettings LevelSettings;
        public IBlockCreator BlockCreator;
        public IExplosionManager ExplosionManager;
        public EnvironmentInfoConfig EnvironmentInfoConfig;
    }
}