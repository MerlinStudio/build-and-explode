using System.Collections.Generic;
using Base.Dev.Core.Runtime.Configs;
using Common.Configs;
using Common.Saves.Interfaces;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
using State.Creator.Controllers;
using State.Creator.Interfaces;
using State.Explosion.Interfaces;
using State.LevelLoader.Providers;
using State.SavaLoader.Controllers;
using Zenject;

namespace Game.Core.GameStateMachine
{
    public class GameStateSwitcher : IInitializerGameState, IGameStateSwitcher, ITickable
    {
        public GameStateSwitcher(
            UiManager uiManager,
            EnvironmentInfoConfig environmentInfoConfig,
            ShopConfig shopConfig,
            LevelsConfig levelsConfig,
            LevelMapConfig levelMapConfig,
            IManagerCreator managerCreator,
            ISavesProvider savesProvider)
        {
            m_uiManager = uiManager;
            m_environmentInfoConfig = environmentInfoConfig;
            m_shopConfig = shopConfig;
            m_levelsConfig = levelsConfig;
            m_levelMapConfig = levelMapConfig;
            m_managerCreator = managerCreator;
            m_savesProvider = savesProvider;
        }
        
        private readonly UiManager m_uiManager;
        private readonly EnvironmentInfoConfig m_environmentInfoConfig;
        private readonly ShopConfig m_shopConfig;
        private readonly LevelsConfig m_levelsConfig;
        private readonly LevelMapConfig m_levelMapConfig;
        private readonly IManagerCreator m_managerCreator;
        private readonly ISavesProvider m_savesProvider;
        
        private IGameTick m_currentGameTicks;
        private AbstractStateBase m_currentState;
        private List<AbstractStateBase> m_allStates;

        public void Init()
        {
            var levelProvider = new LevelProvider();
            var buildCreator = new СonstructionСontroller(levelProvider, m_managerCreator, m_savesProvider, m_environmentInfoConfig.BuildAnimationInfo);
            var saveConstruction = new SaveConstructionController(levelProvider, buildCreator, m_savesProvider);
                        
            var stateLevelLoaderDependencies = new StateLevelLoader.StateLevelLoaderDependencies
            {
                GameStateSwitcher = this,
                SavesProvider = m_savesProvider,
                LevelProvider = levelProvider,
                LevelsConfig = m_levelsConfig,
                ConstructionReset = buildCreator
            };
            var stateSaveLoaderDependencies = new StateSaveLoader.StateSaveLoaderDependencies
            {
                GameStateSwitcher = this,
                UiManager = m_uiManager,
                SaveConstructionController = saveConstruction
            };
            var stateBuildDependencies = new StateBuild.StateBuildDependencies
            {
                GameStateSwitcher = this,
                UiManager = m_uiManager,
                BuildCreator = buildCreator,
                ManagerCreator = m_managerCreator,
                SavesProvider = m_savesProvider,
                EnvironmentInfoConfig = m_environmentInfoConfig,
                LevelProvider = levelProvider,
            };
            var stateExplosionDependencies = new StateExplosion.StateExplosionDependencies
            {
                GameStateSwitcher = this,
                UiManager = m_uiManager,
                EnvironmentInfoConfig = m_environmentInfoConfig,
                ShopConfig = m_shopConfig,
                LevelProvider = levelProvider,
                ManagerCreator = m_managerCreator,
                BlocksInfoProvider = buildCreator
            };
            var stateResultDependencies = new StateResult.StateResultDependencies
            {
                GameStateSwitcher = this,
                UiManager = m_uiManager,
                LevelsConfig = m_levelsConfig,
                LevelMapConfig = m_levelMapConfig,
                SavesProvider = m_savesProvider
            };
            m_allStates = new List<AbstractStateBase>
            {
                new StateLevelLoader(stateLevelLoaderDependencies),
                new StateSaveLoader(stateSaveLoaderDependencies),
                new StateBuild(stateBuildDependencies),
                new StateExplosion(stateExplosionDependencies),
                new StateResult(stateResultDependencies)
            };
            SwitchState<StateLevelLoader>();
        }
        
        public void DeInit()
        {
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
}