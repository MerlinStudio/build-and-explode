using Data.Builds.Configs;
using State.LevelLoader.Interfaces;

namespace State.LevelLoader.Providers
{
    public class LevelProvider : ILevelProvider
    {
        private BuildDataConfig m_currentBuildDataConfig;
        
        public void SetCurrentBuildDataConfig(BuildDataConfig buildDataConfig)
        {
            m_currentBuildDataConfig = buildDataConfig;
        }

        public BuildDataConfig GetCurrentBuildDataConfig()
        {
            return m_currentBuildDataConfig;
        }
    }
}