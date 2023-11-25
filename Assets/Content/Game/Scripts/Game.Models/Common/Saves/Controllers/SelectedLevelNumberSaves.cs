using Common.Saves.Interfaces;
using YG;

namespace Common.Saves.Controllers
{
    public class SelectedLevelNumberSaves : ISaves
    {
        public void SetSavesData(int saveValue)
        {
            YandexGame.savesData.SelectedLevelNumber = saveValue;
            YandexGame.SaveProgress();
        }

        public int GetSavesData()
        {
            return YandexGame.savesData.SelectedLevelNumber;
        }
    }
}