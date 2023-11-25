using System;
using System.Collections.Generic;
using Common.Configs;
using Common.Creators;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Data.Builds.Configs;
using State.Creator.Interfaces;
using State.LevelLoader.Interfaces;
using UnityEngine;

namespace State.Creator.Controllers
{
    public class СonstructionСontroller : IBuildCreator, IBlocksInfoProvider, IConstructionReset
    {
        public СonstructionСontroller(
            ILevelProvider levelProvider,
            IManagerCreator managerCreator,
            ISavesProvider savesProvider,
            BuildAnimationInfo buildAnimationInfo)
        {
            m_levelProvider = levelProvider;
            m_managerCreator = managerCreator;
            m_savesProvider = savesProvider;
            m_buildAnimationInfo = buildAnimationInfo;
        }

        public event Action EventEndConstruction;
        public bool IsAllAnimationFinished => m_blockAnimationController.IsAllAnimationFinished;

        private readonly BuildAnimationInfo m_buildAnimationInfo;
        private readonly ILevelProvider m_levelProvider;
        private readonly IManagerCreator m_managerCreator;
        private readonly ISavesProvider m_savesProvider;

        private BuildDataConfig m_buildDataConfig;
        private BlockAnimationController m_blockAnimationController;
        private BlockViewInfo[] m_blockViewInfo;
        private BlockPropertyInfo[] m_blockPropertyInfo;
        private Dictionary<string, List<NewBlockInfo>> m_blockGameObjectPool;

        private int m_index;
        private int m_amountBlocks;

        public void Init()
        {
            m_blockGameObjectPool ??= new Dictionary<string, List<NewBlockInfo>>();
            m_blockAnimationController = new BlockAnimationController(m_buildAnimationInfo, m_managerCreator);
            m_blockAnimationController.Init();
        }

        public void DeInit()
        {
            m_blockAnimationController.DeInit();
        }
        
        public void ResetBuildData()
        {
            ResetPoolObject();
            m_index = 0;
            m_buildDataConfig = m_levelProvider.GetCurrentBuildDataConfig();
            m_blockViewInfo = new BlockViewInfo[m_buildDataConfig.BlockData.Count];
            m_blockPropertyInfo = new BlockPropertyInfo[m_buildDataConfig.BlockData.Count];
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
                var blockTransform = await CreateBlock();
                blockTransformList.Add(blockTransform);
                m_savesProvider.SetSavesData<LastNumberBlockSaves>(m_index);
                if (m_buildDataConfig.BlockData.Count <= m_index)
                {
                    EventEndConstruction?.Invoke();
                    break;
                }
            }
            m_blockAnimationController.PlayAnimation(blockTransformList);
        }

        public async UniTask<Transform> CreateBlock()
        {
            var blockData = m_buildDataConfig.BlockData[m_index];
            var newBlockInfo = PopBlock(blockData.Id) ?? await m_managerCreator.Create<NewBlockInfo, BlockCreator>(blockData.Id);
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

            PushBlocks(blockData.Id, newBlockInfo);
            return newBlockInfo.Block.transform;
        }

        private NewBlockInfo PopBlock(string blockId)
        {
            if (!m_blockGameObjectPool.ContainsKey(blockId))
            {
                m_blockGameObjectPool[blockId] = new List<NewBlockInfo>();
                return null;
            }
            if (m_blockGameObjectPool[blockId].Count <= 0)
            {
                return null;
            }
            var blockInfo = m_blockGameObjectPool[blockId].Find(i => !i.Block.gameObject.activeSelf);
            blockInfo?.Block.gameObject.SetActive(true);
            return blockInfo;
        }

        private void PushBlocks(string blockId, NewBlockInfo blockInfo)
        {
            if (!m_blockGameObjectPool.ContainsKey(blockId))
            {
                m_blockGameObjectPool[blockId] = new List<NewBlockInfo>();
            }
            m_blockGameObjectPool[blockId].Add(blockInfo);
        }

        private void ResetPoolObject()
        {
            if (m_blockGameObjectPool == null)
            {
                m_blockGameObjectPool = new Dictionary<string, List<NewBlockInfo>>();
                return;
            }
            foreach (var blocksInfo in m_blockGameObjectPool)
            {
                for (int i = 0; i < blocksInfo.Value.Count; i++)
                {
                    var block = blocksInfo.Value[i].Block;
                    block.gameObject.SetActive(false);
                    block.transform.localPosition = Vector3.zero;
                    block.transform.localEulerAngles = Vector3.zero;
                }
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