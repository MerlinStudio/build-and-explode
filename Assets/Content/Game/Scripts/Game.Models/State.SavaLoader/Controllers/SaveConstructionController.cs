using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using State.Creator.Interfaces;
using State.SavaLoader.Interfaces;

namespace State.SavaLoader.Controllers
{
    public class SaveConstructionController : ISaveConstructionController
    {
        public SaveConstructionController(
            BuildDataConfig buildDataConfig,
            IBuildCreator buildCreator,
            int lastPermanentBlockIndex)
        {
            m_buildDataConfig = buildDataConfig;
            m_buildCreator = buildCreator;
            m_lastPermanentBlockIndex = lastPermanentBlockIndex;
        }

        private readonly BuildDataConfig m_buildDataConfig;
        private readonly IBuildCreator m_buildCreator;
        private readonly int m_lastPermanentBlockIndex;

        public async UniTask Construction() 
        {
            for (int j = 0; j < m_lastPermanentBlockIndex; j++)
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
            return m_buildDataConfig.BlockData.Count <= m_lastPermanentBlockIndex;
        }
    }
}