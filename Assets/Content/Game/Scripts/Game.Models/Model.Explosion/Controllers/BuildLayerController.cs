using System;
using System.Collections.Generic;
using System.Linq;
using Content.Game.Scripts.Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Data.Builds.Blocks;
using Model.Creator.Creators;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using UniRx;
using UnityEngine;

namespace Model.Explosion.Controllers
{
    public class BuildLayerController : IBlocksInfoProvider
    {
        public BuildLayerController(
            IManagerCreator managerCreator,
            IBlocksInfoProvider blocksInfoProvider,
            IBombInfoProvider bombInfoProvider)
        {
            m_managerCreator = managerCreator;
            m_blockPropertyInfo = blocksInfoProvider.GetBlockPropertyInfo().ToList();
            m_blockViewInfo = blocksInfoProvider.GetBlockViewInfo().ToList();
            m_bombInfoProvider = bombInfoProvider;
        }

        public event Action<Transform> EventSelectBombPlace;
        
        private readonly string m_selectionBombId = "selection_bomb";
        private readonly IManagerCreator m_managerCreator;
        private readonly IBombInfoProvider m_bombInfoProvider;

        private int m_currentBuildLayer;
        private PointerHandler m_selectionBombReference;
        private CompositeDisposable m_compositeDisposable;
        private CompositeDisposable m_newBombInfoDisposable;
        private NewBlockInfo m_newBombInfo;
        private List<PointerHandler> m_selectionBomb;
        private List<BlockViewInfo> m_blockViewInfo;
        private List<BlockPropertyInfo> m_blockPropertyInfo;
        private Dictionary<int, List<Vector3>> m_selectionBombDictionary;
        private Dictionary<int, List<Transform>> m_addedBombDictionary;
        private Dictionary<int, List<Transform>> m_buildingLayerDictionary;
        private ComponentPool<byte, PointerHandler> m_selectionBombPool;

        public async void Init()
        {
            SetBuildingLayer();
            await SetSelectionBombReference();
            SetSelectionBombPosition();
            ChangeBuildingLayer(1f);
            m_newBombInfoDisposable = new CompositeDisposable();
            m_bombInfoProvider.NewBombInfo.Subscribe(UpdateBombInfo).AddTo(m_newBombInfoDisposable);
        }

        public void DeInit()
        {
            m_compositeDisposable.Dispose();
            m_newBombInfoDisposable.Dispose();
            m_selectionBomb.Clear();
            m_blockViewInfo.Clear();
            m_blockPropertyInfo.Clear();
            m_selectionBombDictionary.Clear();
            m_addedBombDictionary.Clear();
            m_buildingLayerDictionary.Clear();
        }

        private void SetBuildingLayer()
        {
            m_buildingLayerDictionary = new Dictionary<int, List<Transform>>();
            var maxHigh = m_blockViewInfo.Max(v => v.Coord.y);
            for (int i = 0; i <= maxHigh; i++)
            {
                m_buildingLayerDictionary[i] = new List<Transform>();
            }
            for (int i = 0; i < m_blockViewInfo.Count; i++)
            {
                var blockViewInfo = m_blockViewInfo[i];
                m_buildingLayerDictionary[blockViewInfo.Coord.y].Add(blockViewInfo.Transform);
            }
        }
        
        private async UniTask SetSelectionBombReference()
        {
            m_compositeDisposable = new CompositeDisposable();
            m_selectionBomb = new List<PointerHandler>();
            m_selectionBombPool = new ComponentPool<byte, PointerHandler>();
            var newBlockInfo = await m_managerCreator.Create<NewBlockInfo, BlockCreator>(m_selectionBombId);
            m_selectionBombReference = newBlockInfo.Block.GetComponent<PointerHandler>();
            m_selectionBombReference.gameObject.SetActive(false);
        }

        private void SetSelectionBombPosition()
        {
            var capacity = m_buildingLayerDictionary.Count;
            m_selectionBombDictionary = new Dictionary<int, List<Vector3>>(capacity: capacity);
            m_addedBombDictionary = new Dictionary<int, List<Transform>>();
            for (int i = 0; i < capacity; i++)
            {
                m_selectionBombDictionary[i] = new List<Vector3>();
                m_addedBombDictionary[i] = new List<Transform>();
            }
            for (int y = 0; y < capacity; y++)
            {
                var layerBlocksPosition = new List<Vector3>();
                var layerBlocks = m_buildingLayerDictionary[y];
                var maxZ = (int)layerBlocks.Max(t => t.position.z);
                var minZ = (int)layerBlocks.Min(t => t.position.z);
                for (int z = minZ; z <= maxZ; z++)
                {
                    var xBlocks = layerBlocks.Where(t => (int)t.position.z == z).ToList();
                    if (xBlocks.Count <= 0)
                    {
                        continue;
                    }
                    var maxX = (int)xBlocks.Max(t => t.position.x);
                    var minX = (int)xBlocks.Min(t => t.position.x);
                    for (int x = minX; x <= maxX; x++)
                    {
                        layerBlocksPosition.Add(new Vector3(x, y, z));
                    }
                }

                m_selectionBombDictionary[y] = layerBlocksPosition;
            }
        }

        private void UpdateBombInfo(NewBlockInfo newBlockInfo)
        {
            m_newBombInfo = newBlockInfo;
        }

        public void AddBombToLayer()
        {
            var blockViewInfo = m_blockViewInfo.FirstOrDefault(b => b.Transform.position == m_newBombInfo.Block.transform.position);
            if (blockViewInfo != null)
            {
                var oldBlockTransform = m_buildingLayerDictionary[blockViewInfo.Coord.y].Find(b => b.transform == blockViewInfo.Transform);
                oldBlockTransform.gameObject.SetActive(false);
                m_buildingLayerDictionary[blockViewInfo.Coord.y].Remove(oldBlockTransform);
                m_buildingLayerDictionary[blockViewInfo.Coord.y].Add(m_newBombInfo.Block.transform);
                blockViewInfo.Transform = m_newBombInfo.Block.transform;
            }
            else
            {
                var bombPosition = m_newBombInfo.Block.transform.position;
                var coord = new Vector3Int(Mathf.RoundToInt(bombPosition.x), Mathf.RoundToInt(bombPosition.y), Mathf.RoundToInt(bombPosition.z));
                blockViewInfo = new BlockViewInfo
                {
                    Index = m_blockViewInfo.Count,
                    Coord = coord,
                    Transform = m_newBombInfo.Block.transform
                };
                m_blockViewInfo.Add(blockViewInfo);
                m_blockPropertyInfo.Add(m_newBombInfo.Info);
                m_buildingLayerDictionary[blockViewInfo.Coord.y].Add(m_newBombInfo.Block.transform);
                SetBuildingLayer();
            }
            var oldSelectionBomb = m_selectionBomb.Find(b => b.transform.position == blockViewInfo.Coord);
            m_selectionBomb.Remove(oldSelectionBomb);
            m_selectionBombPool.Push(oldSelectionBomb);
            m_selectionBombDictionary[blockViewInfo.Coord.y].Remove(blockViewInfo.Coord);
            m_addedBombDictionary[blockViewInfo.Coord.y].Add(m_newBombInfo.Block.transform);
        }

        public void Explosion()
        {
            ResetAllBlocks();
        }

        public void ChangeBuildingLayer(float value)
        {
            var noRoundValue = value / (1f / (m_buildingLayerDictionary.Count - 1));
            var newBuildLayer = Mathf.RoundToInt(noRoundValue);
            if (m_currentBuildLayer == newBuildLayer)
            {
                return;
            }

            ResetAllBlocks();
            
            for (int i = 0; i < m_buildingLayerDictionary.Count; i++)
            {
                var layerTransformBlocks = m_buildingLayerDictionary[i];
                var selectionPositionBombs = m_selectionBombDictionary[i];
                var addedBombTransforms = m_addedBombDictionary[i];

                if (i >= newBuildLayer)
                {
                    layerTransformBlocks.ForEach(t => t.gameObject.SetActive(false));
                }
                if (i == newBuildLayer)
                {
                    for (int j = 0; j < selectionPositionBombs.Count; j++)
                    {
                        var selectionPositionBomb = selectionPositionBombs[j];
                        var selectionBomb = m_selectionBombPool.Pop(m_selectionBombReference);
                        selectionBomb.transform.position = selectionPositionBomb;
                        selectionBomb.EventItemClick.Subscribe(EventSelectBombPlace).AddTo(m_compositeDisposable);
                        m_selectionBomb.Add(selectionBomb);
                    }
                    addedBombTransforms.ForEach(t => t.gameObject.SetActive(true));
                }
            }
            m_buildingLayerDictionary.Last().Value.ForEach(t => t.gameObject.SetActive(false));

            m_currentBuildLayer = newBuildLayer;
            m_newBombInfo?.Block.gameObject.SetActive(false);
            m_bombInfoProvider.VisibleNewBombInfo.Value = m_newBombInfo != null && m_newBombInfo.Block.gameObject.activeSelf;
        }

        private void ResetAllBlocks()
        {
            for (int i = 0; i < m_buildingLayerDictionary.Count; i++)
            {
                var layerBlocks = m_buildingLayerDictionary[i];
                layerBlocks.ForEach(t => t.gameObject.SetActive(true));
            }

            for (int i = 0; i < m_selectionBomb.Count; i++)
            {
                var selectionBomb = m_selectionBomb[i];
                m_selectionBombPool.Push(selectionBomb);
            }
            m_selectionBomb.Clear();
            m_compositeDisposable.Clear();
        }

        public BlockViewInfo[] GetBlockViewInfo()
        {
            return m_blockViewInfo.ToArray();
        }

        public BlockPropertyInfo[] GetBlockPropertyInfo()
        {
            return m_blockPropertyInfo.ToArray();
        }
    }
}