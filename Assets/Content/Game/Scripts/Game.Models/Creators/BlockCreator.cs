using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Data.Builds.Configs;
using Game.Utils.Extantions;
using State.Creator.Interfaces;
using UnityEngine;
using Zenject;

namespace Creators
{
    public class BlockCreator : MonoBehaviour, ICreator
    {
        [SerializeField] private Transform m_parent;
        
        [Inject] private BlockInfoConfigs BlockInfoConfigs { get; }

        private Dictionary<string, NewBlockInfo> m_newBlockInfoDictionary;
        
        public void Init()
        {
            m_newBlockInfoDictionary ??= new Dictionary<string, NewBlockInfo>();
        }

        public void DeInit()
        {
        }

        public async UniTask<T> Create<T>(string id) where T : CreatedItem
        {
            NewBlockInfo newBlockInfo;
            if (m_newBlockInfoDictionary.TryGetValue(id, out var value))
            {
                newBlockInfo = new NewBlockInfo
                {
                    Block = Instantiate(value.Block, m_parent),
                    Info = value.Info
                };
            }
            else
            {
                var blockInfo = BlockInfoConfigs.BlockInfoList.Find(i => i.Id == id);
                var blockReference = await AssetReferenceExtension.LoadAssetReferenceAsync(blockInfo.BlockReference);
                newBlockInfo = new NewBlockInfo
                {
                    Block = Instantiate(blockReference, m_parent),
                    Info = blockInfo.BlockPropertyInfo
                };
                m_newBlockInfoDictionary[id] = newBlockInfo;
            }

            return newBlockInfo as T;
        }
    }
    
    public class NewBlockInfo : CreatedItem
    {
        public GameObject Block;
        public BlockPropertyInfo Info;
    }
}