using Dev.Core.Level;

namespace Dev.Core.Interfaces
{
    public interface ILevelsProvider
    {
        Level.Level Level { get; set; }
        LevelData GetLevelData();
    }
}