using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Data.Builds.Configs;
using Game.Utils.Extantions;
using Model.Creator.Interfaces;
using UnityEngine;
using Zenject;

namespace Model.Creator.Controllers
{
    public class BlockCreator : MonoBehaviour, IBlockCreator
    {
        [SerializeField] private Transform m_parentBlocks;
        
        [Inject] private BlockInfoConfigs BlockInfoConfigs { get; }

        private Dictionary<string, NewBlockInfo> m_newBlockInfoDictionary;

        public void Init()
        {
            m_newBlockInfoDictionary ??= new Dictionary<string, NewBlockInfo>();
        }

        public void DeInit()
        {
        }

        public async UniTask<NewBlockInfo> Create(string id)
        {
            NewBlockInfo newBlockInfo;
            if (m_newBlockInfoDictionary.TryGetValue(id, out var value))
            {
                newBlockInfo = new NewBlockInfo
                {
                    Block = Instantiate(value.Block, m_parentBlocks),
                    Info = value.Info
                };
            }
            else
            {
                var blockInfo = BlockInfoConfigs.BlockInfoList.Find(i => i.Id == id);
                var blockReference = await AssetReferenceExtension.LoadAssetReferenceAsync(blockInfo.BlockReference);
                newBlockInfo = new NewBlockInfo
                {
                    Block = Instantiate(blockReference, m_parentBlocks),
                    Info = blockInfo.BlockPropertyInfo
                };
                m_newBlockInfoDictionary[id] = newBlockInfo;
            }

            return newBlockInfo;
        }
        
        public class NewBlockInfo
        {
            public GameObject Block;
            public BlockPropertyInfo Info;
        }
    }
}