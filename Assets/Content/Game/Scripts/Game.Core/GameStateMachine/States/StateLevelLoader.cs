using Base.Dev.Core.Runtime.Configs;
using Common.Saves.Interfaces;
using State.Creator.Interfaces;
using State.LevelLoader.Controllers;
using State.LevelLoader.Interfaces;

namespace Game.Core.GameStateMachine
{
    public class StateLevelLoader : AbstractStateBase
    {
        public StateLevelLoader(StateLevelLoaderDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }
        
        private readonly StateLevelLoaderDependencies m_dependencies;

        private LevelLoaderController m_levelLoaderController;

        public override void InitState()
        {
            m_levelLoaderController ??= new LevelLoaderController(
                m_dependencies.LevelsConfig,
                m_dependencies.SavesProvider,
                m_dependencies.LevelProvider);

            if (m_levelLoaderController.CheckCurrentBuildDataConfig())
            {
                m_levelLoaderController.SetCurrentBuildDataConfig();
                m_dependencies.ConstructionReset.ResetBuildData();
                m_dependencies.GameStateSwitcher.SwitchState<StateBuild>();
                return;
            }
            m_levelLoaderController.SetCurrentBuildDataConfig();
            m_dependencies.ConstructionReset.ResetBuildData();
            m_dependencies.GameStateSwitcher.SwitchState<StateSaveLoader>();
        }

        public override void DeinitState()
        {
        }
        
        public class StateLevelLoaderDependencies : StateDependencies
        {
            public IGameStateSwitcher GameStateSwitcher;
            public ISavesProvider SavesProvider;
            public ILevelProvider LevelProvider;
            public IConstructionReset ConstructionReset;
            public LevelsConfig LevelsConfig;
        }
    }
}