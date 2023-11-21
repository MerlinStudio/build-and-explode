using System;
using System.Collections.Generic;
using Configs;
using Data.Explosion.Info;
using Model.Creator.Creators;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using UnityEngine;

namespace Model.Explosion.Controllers
{
    public class PreparationExplosion : IGameTick, IDisposable
    {
        public PreparationExplosion(
            IManagerCreator managerCreator,
            IBlocksInfoProvider blocksInfoProvider,
            EnvironmentInfoConfig environmentInfoConfig,
            IBombInfoProvider bombInfoProvider)
        {
            m_managerCreator = managerCreator;
            m_blocksInfoProvider = blocksInfoProvider;
            m_environmentInfo = environmentInfoConfig.EnvironmentInfo;
            m_bombInfoProvider = bombInfoProvider;
        }
        
        private readonly string m_bombId = "bomb";

        private readonly IManagerCreator m_managerCreator;
        private readonly IBlocksInfoProvider m_blocksInfoProvider;
        private readonly IBombInfoProvider m_bombInfoProvider;
        private readonly EnvironmentInfo m_environmentInfo;
        private ExplosionController m_explosionController;
        private NewBlockInfo m_newBombInfo;
        private Dictionary<ExplosionInfo, Transform> m_explosionInfo;

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
            m_explosionInfo ??= new Dictionary<ExplosionInfo, Transform>();
            m_explosionInfo[explosionInfo] = m_newBombInfo.Block.transform;
            m_newBombInfo = null;
            m_bombInfoProvider.NewBombInfo.Value = null;
            m_bombInfoProvider.VisibleNewBombInfo.Value = false;
            Debug.Log("Bomb info added");
        }

        public void Explosion()
        {
            var blockViewInfo  = m_blocksInfoProvider.GetBlockViewInfo();
            var blockPropertyInfo = m_blocksInfoProvider.GetBlockPropertyInfo();
            m_explosionController = new ExplosionController(blockPropertyInfo, blockViewInfo, m_environmentInfo);
            m_explosionController.Init();
            m_explosionController.SetupTransforms();

            foreach (var explosionInfo in m_explosionInfo)
            {
                m_explosionController.Explosion(explosionInfo.Key);
                explosionInfo.Value.gameObject.SetActive(false);
            }
        }

        public async void OnSelectBombPlace(Transform selectionBombPlace)
        {
            m_newBombInfo?.Block.gameObject.SetActive(true);
            m_newBombInfo ??= await m_managerCreator.Create<NewBlockInfo, BlockCreator>(m_bombId);
            m_newBombInfo.Block.transform.position = selectionBombPlace.position;
            m_bombInfoProvider.NewBombInfo.Value = m_newBombInfo;
            m_bombInfoProvider.VisibleNewBombInfo.Value = true;
        }

        public void Tick()
        {
            m_explosionController?.Update();
        }

        public void Dispose()
        {
            m_explosionInfo.Clear();
        }
    }
}