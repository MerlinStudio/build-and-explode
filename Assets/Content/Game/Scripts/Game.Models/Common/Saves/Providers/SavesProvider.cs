using System.Collections.Generic;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using YG;
using Zenject;

namespace Common.Saves.Providers
{
    public class SavesProvider : ISavesProvider, IInitializerSaves
    {
        private List<ISaves> m_saves;
        private bool m_isSDKLoaded;

        [Inject]
        private void Init()
        {
            m_saves = new List<ISaves>
            {
                new LastNumberBlockSaves(),
                new PastLevelNumberSaves(),
                new SelectedLevelNumberSaves(),
                new BlockMultiplierSaves()
            };
        }
        
        public void SetSavesData<T>(int saveValue) where T : ISaves
        {
            var saves = m_saves.Find(s => s is T);
            saves.SetSavesData(saveValue);
        }

        public int GetSavesData<T>() where T : ISaves
        {
            var saves = m_saves.Find(s => s is T);
            return saves.GetSavesData();
        }

        public UniTask WaitInitYandexSDK()
        {
            return UniTask.WaitWhile(() => YandexGame.SDKEnabled == false);
        }
    }
}