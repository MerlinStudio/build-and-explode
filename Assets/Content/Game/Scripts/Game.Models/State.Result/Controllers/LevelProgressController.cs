using Common.Saves.Controllers;
using Common.Saves.Interfaces;

namespace State.Result.Controllers
{
    public class LevelProgressController
    {
        public LevelProgressController(ISavesProvider savesProvider)
        {
            m_savesProvider = savesProvider;
        }
        
        private readonly ISavesProvider m_savesProvider;

        public void UpdateProgressLevel()
        {
            var pastLevel = m_savesProvider.GetSavesData<PastLevelNumberSaves>();
            var selectedLevel = m_savesProvider.GetSavesData<SelectedLevelNumberSaves>();
            if (pastLevel == selectedLevel)
            {
                pastLevel++;
            }
            selectedLevel = pastLevel;
            m_savesProvider.SetSavesData<PastLevelNumberSaves>(pastLevel);
            m_savesProvider.SetSavesData<SelectedLevelNumberSaves>(selectedLevel);
            m_savesProvider.SetSavesData<LastNumberBlockSaves>(0);
        }
    }
}