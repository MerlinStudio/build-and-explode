using System;
using System.Collections.Generic;
using System.Linq;
using Common.Configs;
using Common.Creators;
using Cysharp.Threading.Tasks;
using Data.Explosion.Info;
using State.Creator.Interfaces;
using State.Explosion.Interfaces;
using UniRx;
using UnityEngine;

namespace State.Explosion.Controllers
{
    public class PreparationExplosion : IGameTick, IDisposable
    {
        public PreparationExplosion(
            IManagerCreator managerCreator,
            IBlocksInfoProvider blocksInfoProvider,
            EnvironmentInfoConfig environmentInfoConfig,
            IBombInfoProvider bombInfoProvider,
            ISubject<Unit> explosionFinished)
        {
            m_managerCreator = managerCreator;
            m_blocksInfoProvider = blocksInfoProvider;
            m_environmentInfo = environmentInfoConfig.EnvironmentInfo;
            m_bombInfoProvider = bombInfoProvider;
            m_explosionFinished = explosionFinished;
        }
        
        private readonly string m_bombId = "bomb";
        private readonly IManagerCreator m_managerCreator;
        private readonly IBlocksInfoProvider m_blocksInfoProvider;
        private readonly IBombInfoProvider m_bombInfoProvider;
        private readonly ISubject<Unit> m_explosionFinished;
        private readonly EnvironmentInfo m_environmentInfo;
        
        private ExplosionController m_explosionController;
        private NewBlockInfo m_newBombInfo;
        private Dictionary<ExplosionInfo, Transform> m_explosionInfo;
        private Dictionary<ExplosionInfo, float> m_delayExplosionInfo;

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
            m_delayExplosionInfo ??= new Dictionary<ExplosionInfo, float>();
            m_delayExplosionInfo[explosionInfo] = delay;
            m_newBombInfo = null;
            m_bombInfoProvider.NewBombInfo.Value = null;
            m_bombInfoProvider.VisibleNewBombInfo.Value = false;
            Debug.Log("Bomb info added");
        }

        public async void Explosion()
        {
            var blockViewInfo  = m_blocksInfoProvider.GetBlockViewInfo();
            var blockPropertyInfo = m_blocksInfoProvider.GetBlockPropertyInfo();
            m_explosionController = new ExplosionController(blockPropertyInfo, blockViewInfo, m_environmentInfo, m_explosionFinished);
            m_explosionController.Init();
            m_explosionController.SetupTransforms();
            var explosionsLeft = m_explosionInfo.Count;
            var sortDelayExplosionInfo = m_delayExplosionInfo.OrderBy(x => x.Value)
                .ToDictionary(k => k.Key, v => v.Value);

            foreach (var delayExplosionInfo in sortDelayExplosionInfo)
            {
                explosionsLeft--;
                m_explosionController.Explosion(delayExplosionInfo.Key, explosionsLeft);
                var bombTransform = m_explosionInfo[delayExplosionInfo.Key];
                bombTransform.gameObject.SetActive(false);
                var delay = (int)(delayExplosionInfo.Value * 1000);
                await UniTask.Delay(delay);
            }
        }

        public async void OnSelectBombPlace(Transform selectionBombPlace)
        {
            m_newBombInfo ??= await m_managerCreator.Create<NewBlockInfo, BlockCreator>(m_bombId);
            m_newBombInfo.Block.transform.localPosition = selectionBombPlace.position;
            m_newBombInfo.Block.transform.localEulerAngles = Vector3.zero;
            m_bombInfoProvider.NewBombInfo.Value = m_newBombInfo;
            m_bombInfoProvider.VisibleNewBombInfo.Value = true;
            m_newBombInfo.Block.gameObject.SetActive(true);
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