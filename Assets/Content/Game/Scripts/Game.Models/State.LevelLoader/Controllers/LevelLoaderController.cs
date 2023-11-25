using System.Collections.Generic;
using System.Linq;
using Base.Dev.Core.Runtime.Configs;
using Base.Dev.Core.Runtime.Level;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using State.LevelLoader.Interfaces;

namespace State.LevelLoader.Controllers
{
    public class LevelLoaderController
    {
        public LevelLoaderController(
            LevelsConfig levelsConfig,
            ISavesProvider savesProvider,
            ILevelProvider levelProvider)
        {
            m_levelsConfig = levelsConfig;
            m_savesProvider = savesProvider;
            m_levelProvider = levelProvider;
        }
        
        private readonly LevelsConfig m_levelsConfig;
        private readonly ISavesProvider m_savesProvider;
        private readonly ILevelProvider m_levelProvider;

        public bool CheckCurrentBuildDataConfig()
        {
            return m_levelProvider.GetCurrentBuildDataConfig() != null;
        }

        public void SetCurrentBuildDataConfig()
        {
            var levelData = GetLevelData();
            m_levelProvider.SetCurrentBuildDataConfig(levelData.LevelSettings.BuildDataConfig);
        }

        private LevelData GetLevelData()
        {
            var selectedLevelNumber = m_savesProvider.GetSavesData<SelectedLevelNumberSaves>();
            var levelsCount = m_levelsConfig.LevelPacks[0].LevelsData.Count;
            if (selectedLevelNumber >= levelsCount)
            {
                var repeatLevels = GetRepeatLevels();
                var repeatIndex = (selectedLevelNumber - levelsCount) % repeatLevels.Count;
                var repeatLevelData = repeatLevels[repeatIndex];
                return repeatLevelData;
            }

            var levelData = m_levelsConfig.LevelPacks[0].LevelsData.ElementAt(selectedLevelNumber);
            return levelData;
        }

        private List<LevelData> GetRepeatLevels()
        {
            return m_levelsConfig.LevelPacks[0].LevelsData.Where(t => t.IsRepeat).ToList();
        }
    }
}