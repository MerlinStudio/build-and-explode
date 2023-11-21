using System;
using System.Collections.Generic;
using Configs;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Data.Builds.Configs;
using Model.Creator.Creators;
using Model.Creator.Interfaces;
using UnityEngine;

namespace Model.Creator.Controllers
{
    public class СonstructionСontroller : IBuildCreator, IBlocksInfoProvider
    {
        public СonstructionСontroller(
            BuildDataConfig buildDataConfig,
            IManagerCreator managerCreator,
            BuildAnimationInfo buildAnimationInfo)
        {
            m_buildDataConfig = buildDataConfig;
            m_managerCreator = managerCreator;
            m_buildAnimationInfo = buildAnimationInfo;
        }

        public event Action EventEndConstruction;
        public bool IsAllAnimationFinished => m_blockAnimationController.IsAllAnimationFinished;

        private readonly BuildDataConfig m_buildDataConfig;
        private readonly BuildAnimationInfo m_buildAnimationInfo;
        private readonly IManagerCreator m_managerCreator;

        private BlockAnimationController m_blockAnimationController;
        private BlockViewInfo[] m_blockViewInfo;
        private BlockPropertyInfo[] m_blockPropertyInfo;
        private int m_index;
        private int m_amountBlocks;

        public void Init(int index)
        {
            m_index = index;
            m_blockViewInfo = new BlockViewInfo[m_buildDataConfig.BlockData.Count];
            m_blockPropertyInfo = new BlockPropertyInfo[m_buildDataConfig.BlockData.Count];
            m_blockAnimationController = new BlockAnimationController(m_buildAnimationInfo, m_managerCreator);
            m_blockAnimationController.Init();
        }

        public void DeInit()
        {
            m_blockAnimationController.DeInit();
        }

        public void SetAmountBlocks(int amountBlocks)
        {
            m_amountBlocks = amountBlocks;
        }

        public async void CreateBlocks()
        {
            var blockTransformList = new List<Transform>();
            for (int i = 0; i < m_amountBlocks; i++)
            {
                if (m_index + i >= m_buildDataConfig.BlockData.Count)
                {
                    break;
                }
                var blockTransform = await CreateBlock();
                blockTransformList.Add(blockTransform);
            }
            m_blockAnimationController.PlayAnimation(blockTransformList);
        }

        private async UniTask<Transform> CreateBlock()
        {
            var blockData = m_buildDataConfig.BlockData[m_index];
            var newBlockInfo = await m_managerCreator.Create<NewBlockInfo, BlockCreator>(blockData.Id);
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
                EventEndConstruction?.Invoke();
            }

            return newBlockInfo.Block.transform;
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