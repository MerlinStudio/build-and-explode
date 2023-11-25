using Common.Saves.Interfaces;
using YG;

namespace Common.Saves.Controllers
{
    public class LastNumberBlockSaves : ISaves
    {
        public void SetSavesData(int saveValue)
        {
            YandexGame.savesData.LastNumberBlock = saveValue;
#if UNITY_EDITOR
            YandexGame.SaveProgress();
#endif
        }

        public int GetSavesData()
        {
            return YandexGame.savesData.LastNumberBlock;
        }
    }
}