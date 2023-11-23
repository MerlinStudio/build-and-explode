using System.Collections.Generic;
using System.Linq;
using Dev.Core.Interfaces;
using Dev.Core.Level;
using Game.Data.Models;

namespace Dev.Core.Main
{
    public class PlayerLevelsProvider : ILevelsProvider
    {
        public PlayerLevelsProvider(GameDataModel gameDataModel, LevelsConfig levelsConfig)
        {
            this.gameDataModel = gameDataModel;
            this.levelsConfig = levelsConfig;
        }
        
        private readonly GameDataModel gameDataModel;
        private readonly LevelsConfig levelsConfig;

        public Level.Level Level { get; set; }

        public LevelData GetLevelData()
        {
            var selectedLevelNumber = gameDataModel.PastLevelNumber;
            var levelsCount = levelsConfig.LevelPacks[0].LevelsData.Count;
            if (selectedLevelNumber >= levelsCount)
            {
                var repeatLevels = GetRepeatLevels();
                var repeatIndex = (selectedLevelNumber - levelsCount) % repeatLevels.Count;
                var repeatLevelData = repeatLevels[repeatIndex];
                return repeatLevelData;
            }

            var levelData = levelsConfig.LevelPacks[0].LevelsData.ElementAt(selectedLevelNumber);
            return levelData;
        }

        private List<LevelData> GetRepeatLevels()
        {
            return levelsConfig.LevelPacks[0].LevelsData.Where(t => t.IsRepeat).ToList();
        }
    }
}