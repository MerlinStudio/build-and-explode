using Common.Saves.Interfaces;
using YG;

namespace Common.Saves.Controllers
{
    public class BlockMultiplierSaves : ISaves
    {
        public void SetSavesData(int saveValue)
        {
            YandexGame.savesData.BlockMultiplier = saveValue;
        }

        public int GetSavesData()
        {
            return YandexGame.savesData.BlockMultiplier;
        }
    }
}