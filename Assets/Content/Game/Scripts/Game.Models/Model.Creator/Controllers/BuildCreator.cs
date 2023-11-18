using System;
using Data.Builds.Blocks;
using Data.Builds.Configs;
using Model.Creator.Interfaces;

namespace Model.Creator.Controllers
{
    public class BuildCreator : IBuildCreator, IBuildProvider
    {
        public BuildCreator(BuildDataConfig buildDataConfig, IBlockCreator blockCreator)
        {
            m_buildDataConfig = buildDataConfig;
            m_blockCreator = blockCreator;
        }
        
        public event Action EventBuildFinished;

        private readonly BuildDataConfig m_buildDataConfig;
        private readonly IBlockCreator m_blockCreator;
        private BlockViewInfo[] m_blockViewInfo;
        private BlockPropertyInfo[] m_blockPropertyInfo;
        private int m_index;

        public void Init(int index)
        {
            m_index = index;
            m_blockCreator.Init();
            m_blockViewInfo = new BlockViewInfo[m_buildDataConfig.BlockData.Count];
            m_blockPropertyInfo = new BlockPropertyInfo[m_buildDataConfig.BlockData.Count];
        }

        public void DeInit()
        {
            m_blockCreator.DeInit();
        }

        public async void CreateBlock()
        {
            var blockData = m_buildDataConfig.BlockData[m_index];
            var newBlockInfo = await m_blockCreator.Create(blockData.Id);
            var blockViewInfo = new BlockViewInfo
            {
                Index = m_index,
                Coord = blockData.Coord,
                Transform = newBlockInfo.Block.transform
            };
            newBlockInfo.Block.transform.position = blockData.Coord;
            newBlockInfo.Info.Index = m_index;
            m_blockViewInfo[m_index] = blockViewInfo;
            m_blockPropertyInfo[m_index] = newBlockInfo.Info;
            m_index++;
            if (m_buildDataConfig.BlockData.Count <= m_index)
            {
                EventBuildFinished?.Invoke();
            }
        }

        public BlockViewInfo[] GetBlockViewInfo()
        {
            return m_blockViewInfo;
        }

        public BlockPropertyInfo[] GetBlockPropertyInfo()
        {
            return m_blockPropertyInfo;
        }
    }
}