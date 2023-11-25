using System.Collections.Generic;
using System.Linq;
using Base.Dev.Core.Runtime.Configs;
using Base.Dev.Core.Runtime.Level;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Dev.Core.Interfaces;
using Dev.Core.Level;

namespace Dev.Core.Main
{
    public class PlayerLevelsProvider : ILevelsProvider
    {
        public PlayerLevelsProvider(LevelsConfig levelsConfig, ISavesProvider savesProvider)
        {
            m_levelsConfig = levelsConfig;
            m_savesProvider = savesProvider;
        }
        
        private readonly LevelsConfig m_levelsConfig;
        private readonly ISavesProvider m_savesProvider;

        public Level.Level Level { get; set; }

        private int m_selectedLevelNumber;
        private bool m_isSavesLoaded;

        public LevelData GetLevelData()
        {
            m_selectedLevelNumber = m_savesProvider.GetSavesData<SelectedLevelNumberSaves>();
            var levelsCount = m_levelsConfig.LevelPacks[0].LevelsData.Count;
            if (m_selectedLevelNumber >= levelsCount)
            {
                var repeatLevels = GetRepeatLevels();
                var repeatIndex = (m_selectedLevelNumber - levelsCount) % repeatLevels.Count;
                var repeatLevelData = repeatLevels[repeatIndex];
                return repeatLevelData;
            }

            var levelData = m_levelsConfig.LevelPacks[0].LevelsData.ElementAt(m_selectedLevelNumber);
            return levelData;
        }

        private List<LevelData> GetRepeatLevels()
        {
            return m_levelsConfig.LevelPacks[0].LevelsData.Where(t => t.IsRepeat).ToList();
        }
    }
}