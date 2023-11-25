using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using State.Creator.Interfaces;
using State.LevelLoader.Interfaces;
using State.SavaLoader.Interfaces;
using UnityEngine;

namespace State.SavaLoader.Controllers
{
    public class SaveConstructionController : ISaveConstructionController
    {
        public SaveConstructionController(
            ILevelProvider levelProvider,
            IBuildCreator buildCreator,
            ISavesProvider savesProvider)
        {
            m_levelProvider = levelProvider;
            m_buildCreator = buildCreator;
            m_savesProvider = savesProvider;
        }

        private readonly ILevelProvider m_levelProvider;
        private readonly IBuildCreator m_buildCreator;
        private readonly ISavesProvider m_savesProvider;

        private int m_lastNumberBlockSaves;

        public async UniTask Construction()
        {
            m_lastNumberBlockSaves = m_savesProvider.GetSavesData<LastNumberBlockSaves>();
            for (int j = 0; j < m_lastNumberBlockSaves; j++)
            {
                await m_buildCreator.CreateBlock();
                if (j % 10 == 0)
                {
                    await UniTask.WaitForFixedUpdate();
                } 
            }
        }

        public bool CheckConstructionProgress()
        {
            var buildDataConfig = m_levelProvider.GetCurrentBuildDataConfig();
            return buildDataConfig.BlockData.Count <= m_lastNumberBlockSaves;
        }
    }
}