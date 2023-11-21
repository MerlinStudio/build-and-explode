using System.Collections.Generic;
using System.Linq;
using Configs;
using Cysharp.Threading.Tasks;
using Game.Utils.Extantions;
using Model.Creator.Interfaces;
using UnityEngine;
using Zenject;

namespace Model.Creator.Creators
{
    public class ParticleCreator : MonoBehaviour, ICreator
    {
        [SerializeField] private Transform m_parent;
        [Inject] private EnvironmentInfoConfig EnvironmentInfoConfig { get; }

        private Dictionary<string, List<ParticleSystem>> m_particlePool;
        
        public void Init()
        {
            m_particlePool ??= new Dictionary<string, List<ParticleSystem>>();
            if (m_particlePool.Count <= 0)
            {
                for (int i = 0, l = EnvironmentInfoConfig.ParticlesInfo.Count; i < l; i++)
                {
                    var particleSystemInfo = EnvironmentInfoConfig.ParticlesInfo[i];
                    m_particlePool[particleSystemInfo.Id] = new List<ParticleSystem>();
                }
            }
        }

        public void DeInit()
        {
        }

        public async UniTask<T> Create<T>(string id) where T : CreatedItem
        {
            ParticleCreatedItem particleCreatedItem = new ParticleCreatedItem();
            if (m_particlePool.Count > 0)
            {
                var particlePool = m_particlePool.FirstOrDefault(p => p.Key == id).Value;
                if (particlePool.Count > 0)
                {
                    var particle = particlePool.Find(p => !p.gameObject.activeSelf);
                    if (particle != null)
                    {
                        particle.gameObject.SetActive(true);
                        particleCreatedItem.Particle = particle;
                        return particleCreatedItem as T;
                    }
                }
            }
            var particleInfo = EnvironmentInfoConfig.ParticlesInfo.FirstOrDefault(p => p.Id == id);
            var particleReference = await AssetReferenceExtension.LoadAssetReferenceAsync(particleInfo.ParticleReference);
            var newParticle = Instantiate(particleReference, m_parent).GetComponent<ParticleSystem>();
            m_particlePool[id].Add(newParticle);
            particleCreatedItem.Particle = newParticle;
            return particleCreatedItem as T;
        }
    }
    
    public class ParticleCreatedItem : CreatedItem
    {
        public ParticleSystem Particle;
    }
}