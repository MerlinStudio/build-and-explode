using System;
using Dev.Core.Level;

namespace Dev.Core.Interfaces
{
    public interface ILevelLoader
    {
        Level.Level CurrentLevel { get; }

        void LoadLevel(LevelData levelData, Action<Level.Level> onLoaded = null);
        void UnloadCurrentLevel();
    }
}