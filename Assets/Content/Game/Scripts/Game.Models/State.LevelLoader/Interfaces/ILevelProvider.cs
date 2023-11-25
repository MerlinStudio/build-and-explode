using Data.Builds.Configs;

namespace State.LevelLoader.Interfaces
{
    public interface ILevelProvider
    {
        void SetCurrentBuildDataConfig(BuildDataConfig buildDataConfig);
        BuildDataConfig GetCurrentBuildDataConfig();
    }
}