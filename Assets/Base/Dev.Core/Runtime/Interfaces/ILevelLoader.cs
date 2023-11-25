using System;
using Base.Dev.Core.Runtime.Level;
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