using Base.Dev.Core.Runtime.Configs;
using Common.Saves.Interfaces;
using Game.Core.GameStateMachine.Interfaces;
using State.Creator.Interfaces;
using State.LevelLoader.Controllers;
using State.LevelLoader.Interfaces;

namespace Game.Core.GameStateMachine.States
{
    public class StateLevelLoader : AbstractStateBase
    {
        public StateLevelLoader(
            IGameStateSwitcher gameStateSwitcher, 
            ISavesProvider savesProvider,
            ILevelProvider levelProvider,
            IConstructionReset constructionReset,
            LevelsConfig levelsConfig)
        {
            m_gameStateSwitcher = gameStateSwitcher;
            m_savesProvider = savesProvider;
            m_levelProvider = levelProvider;
            m_constructionReset = constructionReset;
            m_levelsConfig = levelsConfig;
        }
        
        private readonly IGameStateSwitcher m_gameStateSwitcher;
        private readonly ISavesProvider m_savesProvider;
        private readonly ILevelProvider m_levelProvider;
        private readonly IConstructionReset m_constructionReset;
        private readonly LevelsConfig m_levelsConfig;
        
        private LevelLoaderController m_levelLoaderController;

        public override void InitState()
        {
            m_levelLoaderController ??= new LevelLoaderController(m_levelsConfig, m_savesProvider, m_levelProvider);
            if (m_levelLoaderController.CheckCurrentBuildDataConfig())
            {
                m_levelLoaderController.SetCurrentBuildDataConfig();
                m_constructionReset.ResetBuildData();
                m_gameStateSwitcher.SwitchState<StateBuild>();
                return;
            }
            m_levelLoaderController.SetCurrentBuildDataConfig();
            m_constructionReset.ResetBuildData();
            m_gameStateSwitcher.SwitchState<StateSaveLoader>();
        }

        public override void DeinitState()
        {
        }
    }
}