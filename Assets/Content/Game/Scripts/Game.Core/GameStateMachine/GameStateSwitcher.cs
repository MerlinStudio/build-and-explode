using System.Collections.Generic;
using Configs;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.Data.Models;
using State.Creator.Controllers;
using State.Creator.Interfaces;
using State.Explosion.Interfaces;
using State.SavaLoader.Controllers;

namespace Game.Core.GameStateMachine
{
    public class GameStateSwitcher : IGameStateSwitcher, IGameTick
    {
        private IGameTick m_currentGameTicks;
        private AbstractStateBase m_currentState;
        private List<AbstractStateBase> m_allStates;

        public void Init(SwitcherDependencies dependencies)
        {
            int lastPermanentBlockIndex = dependencies.LevelSettings.BuildDataConfig.BlockData.Count - 10; // todo взять из сохранения
            var buildCreator = new СonstructionСontroller(
                dependencies.LevelSettings.BuildDataConfig,
                dependencies.ManagerCreator,
                dependencies.EnvironmentInfoConfig.BuildAnimationInfo);
            var saveConstruction = new SaveConstructionController(
                dependencies.LevelSettings.BuildDataConfig,
                buildCreator,
                lastPermanentBlockIndex);
            
            var stateSaveLoaderDependencies = new StateSaveLoader.StateSaveLoaderDependencies
            {
                GameStateSwitcher = this,
                UiManager = dependencies.UiManager,
                SaveConstructionController = saveConstruction
            };
            var stateBuildDependencies = new StateBuild.StateBuildDependencies
            {
                GameStateSwitcher = this,
                UiManager = dependencies.UiManager,
                BuildCreator = buildCreator,
                ManagerCreator = dependencies.ManagerCreator,
                EnvironmentInfoConfig = dependencies.EnvironmentInfoConfig,
                BuildDataConfig = dependencies.LevelSettings.BuildDataConfig,
            };
            var stateExplosionDependencies = new StateExplosion.StateExplosionDependencies
            {
                GameStateSwitcher = this,
                UiManager = dependencies.UiManager,
                EnvironmentInfoConfig = dependencies.EnvironmentInfoConfig,
                ShopConfig = dependencies.ShopConfig,
                BuildDataConfig = dependencies.LevelSettings.BuildDataConfig,
                ManagerCreator = dependencies.ManagerCreator,
                BlocksInfoProvider = buildCreator
            };
            var stateResultDependencies = new StateResult.StateResultDependencies
            {
                GameStateSwitcher = this,
                UiManager = dependencies.UiManager,
                LevelsConfig = dependencies.LevelsConfig,
                LevelMapConfig = dependencies.LevelMapConfig
            };
            m_allStates = new List<AbstractStateBase>
            {
                new StateSaveLoader(stateSaveLoaderDependencies),
                new StateBuild(stateBuildDependencies),
                new StateExplosion(stateExplosionDependencies),
                new StateResult(stateResultDependencies)
            };
            //SwitchState<StateSaveLoader>();
            SwitchState<StateResult>();
        }
        
        public void SwitchState<T>() where T : AbstractStateBase
        {
            var state = m_allStates.Find(s => s is T);
            m_currentState?.DeinitState();
            m_currentState = state;
            m_currentState.InitState();
            m_currentGameTicks = m_currentState;
        }

        public void Tick()
        {
            m_currentGameTicks?.Tick();
        }
    }

    public class SwitcherDependencies
    {
        public UiManager UiManager;
        public GameDataModel GameDataModel;
        public LevelSettings LevelSettings;
        public EnvironmentInfoConfig EnvironmentInfoConfig;
        public ShopConfig ShopConfig;
        public LevelsConfig LevelsConfig;
        public LevelMapConfig LevelMapConfig;
        public IManagerCreator ManagerCreator;
    }
}