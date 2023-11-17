using System.Collections.Generic;
using System.Linq;
using Data.Builds.Blocks;
using Data.Explosion.Configs;
using Data.Explosion.Info;
using Model.Creator.Interfaces;
using Model.Explosion.Interfaces;
using UnityEngine;

namespace Model.Explosion.Controllers
{
    public class ExplosionManager : IExplosionManager, IGameTick
    {
        private IBlockCreator m_blockCreator;
        private ExplosionController m_explosionController;
        private List<ExplosionInfo> m_explosionInfo;
        private BlockViewInfo[] m_blockViewInfo;
        private string m_bombId = "bomb";
        
        public void Init(IBuildProvider buildProvider, IBlockCreator blockCreator, EnvironmentInfoConfig environmentInfoConfig)
        {
            var blockPropertyInfo = buildProvider.GetBlockPropertyInfo();
            m_blockViewInfo = buildProvider.GetBlockViewInfo();
            m_explosionController = new ExplosionController(blockPropertyInfo, m_blockViewInfo, environmentInfoConfig.EnvironmentInfo);
            m_explosionController.Init();
            m_explosionInfo = new List<ExplosionInfo>();
            m_blockCreator = blockCreator;
        }

        public void DeInit()
        {
            m_explosionInfo.Clear();
        }

        public async void AddBomb(Transform blockTransform, int radius, int force, float delay)
        {
            var position = blockTransform.position;
            var explosionInfo = new ExplosionInfo()
            {
                Center = position,
                Radius = radius,
                Force = force
            };
            m_explosionInfo.Add(explosionInfo);
            var bombInfo = await m_blockCreator.Create(m_bombId);
            bombInfo.Block.transform.position = position;
            var blockViewInfo = m_blockViewInfo.FirstOrDefault(b => b.Transform == blockTransform);
            if (blockViewInfo != null)
            {
                blockViewInfo.Transform = bombInfo.Block.transform;
            }
        }

        public void Explosion()
        {
            m_explosionController.SetupTransforms();
            for (int i = 0; i < m_explosionInfo.Count; i++)
            {
                var explosionInfo = m_explosionInfo[i];
                m_explosionController.Explosion(explosionInfo);
            }
        }

        public void Tick()
        {
            m_explosionController.Update();
        }
    }
}