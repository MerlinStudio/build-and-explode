using System.Collections.Generic;
using System.Linq;
using Content.Game.Scripts.Game.Utils.Extensions;
using Data.Builds.Blocks;
using Data.Explosion.Configs;
using Data.Explosion.Info;
using Lean.Touch;
using Model.Creator.Controllers;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Model.Explosion.Controllers
{
    public class ExplosionManager : IExplosionManager, IGameTick
    {
        private readonly string m_bombId = "bomb";
        private readonly string m_selectionBombId = "selection_bomb";

        private IBlockCreator m_blockCreator;
        private ExplosionController m_explosionController;
        private EnvironmentInfo m_environmentInfo;
        private List<ExplosionInfo> m_explosionInfo;
        private List<BlockViewInfo> m_blockViewInfo;
        private List<BlockPropertyInfo> m_blockPropertyInfo;
        private Dictionary<int, List<Transform>> m_buildingLayerDictionary;
        private int m_currentBuildLayer;

        private BlockCreator.NewBlockInfo m_newBombInfo;
        private PointerHandler m_selectionBombReference;
        private CompositeDisposable m_compositeDisposable;
        private List<PointerHandler> m_selectionBomb;
        private ComponentPool<byte, PointerHandler> m_selectionBombPool;
        private Dictionary<int, List<Vector3>> m_selectionBombDictionary;
        private Dictionary<int, List<Transform>> m_addedBombDictionary;

        public void Init(IBuildProvider buildProvider, IBlockCreator blockCreator, EnvironmentInfoConfig environmentInfoConfig)
        {
            m_blockCreator = blockCreator;
            m_environmentInfo = environmentInfoConfig.EnvironmentInfo;
            m_blockPropertyInfo = buildProvider.GetBlockPropertyInfo().ToList();
            m_blockViewInfo = buildProvider.GetBlockViewInfo().ToList();

            m_explosionInfo = new List<ExplosionInfo>();
            SetBuildingLayer();
            SetSelectionBombReference();
            SetSelectionBombPosition();
        }

        public void DeInit()
        {
            m_explosionInfo.Clear();
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

        private async void SetSelectionBombReference()
        {
            m_compositeDisposable = new CompositeDisposable();
            m_selectionBomb = new List<PointerHandler>();
            m_selectionBombPool = new ComponentPool<byte, PointerHandler>();
            var newBlockInfo = await m_blockCreator.Create(m_selectionBombId);
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

        public void AddBombInfo(int radius, int force, float delay)
        {
            if (m_newBombInfo == null)
            {
                return;
            }

            var position = m_newBombInfo.Block.transform.position;
            var explosionInfo = new ExplosionInfo
            {
                Center = position,
                Radius = radius,
                Force = force
            };
            m_explosionInfo.Add(explosionInfo);
            var blockViewInfo = m_blockViewInfo.FirstOrDefault(b => b.Transform.position == position);
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
            m_newBombInfo = null;
            
            Debug.Log("Bomb added");
        }

        public void Explosion()
        {
            ResetAllBlocks();
            
            m_explosionController = new ExplosionController(m_blockPropertyInfo.ToArray(), m_blockViewInfo.ToArray(), m_environmentInfo);
            m_explosionController.Init();
            m_explosionController.SetupTransforms();
            for (int i = 0; i < m_explosionInfo.Count; i++)
            {
                var explosionInfo = m_explosionInfo[i];
                m_explosionController.Explosion(explosionInfo);
            }
        }

        public void ChangeBuildingLayer(float value)
        {
            var noRoundValue = (value / (1f / m_buildingLayerDictionary.Count));
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
                        selectionBomb.EventItemClick.Subscribe(OnSelectionBomb).AddTo(m_compositeDisposable);
                        m_selectionBomb.Add(selectionBomb);
                    }
                    addedBombTransforms.ForEach(t => t.gameObject.SetActive(true));
                }
            }

            m_currentBuildLayer = newBuildLayer;
            m_newBombInfo?.Block.gameObject.SetActive(false);
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
        
        private async void OnSelectionBomb(Transform selectionBlockTransform)
        {
            m_newBombInfo?.Block.gameObject.SetActive(true);
            m_newBombInfo ??= await m_blockCreator.Create(m_bombId);
            m_newBombInfo.Block.transform.position = selectionBlockTransform.position;
        }

        public void Tick()
        {
            if (m_explosionController == null)
            {
                return;
            }
            m_explosionController.Update();
        }
    }
}