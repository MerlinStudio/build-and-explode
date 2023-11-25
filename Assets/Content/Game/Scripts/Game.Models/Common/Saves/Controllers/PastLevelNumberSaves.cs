using Common.Saves.Interfaces;
using YG;

namespace Common.Saves.Controllers
{
    public class PastLevelNumberSaves : ISaves
    {
        public void SetSavesData(int saveValue)
        {
            YandexGame.savesData.PastLevelNumber = saveValue;
            YandexGame.SaveProgress();
        }

        public int GetSavesData()
        {
            return YandexGame.savesData.PastLevelNumber;
        }
    }
}