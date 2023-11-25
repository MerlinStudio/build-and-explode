using Base.Dev.Core.Runtime.Level;
using Cysharp.Threading.Tasks;
using Dev.Core.Level;

namespace Dev.Core.Interfaces
{
    public interface ILevelsProvider
    {
        Level.Level Level { get; set; }
        LevelData GetLevelData();
    }
}