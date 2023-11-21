using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using Model.Creator.Creators;
using Model.Creator.Interfaces;
using UnityEngine;

namespace Model.Creator.Controllers
{
    public class FireworkController
    {
        public FireworkController(
            IManagerCreator managerCreator,
            BuildDataConfig buildDataConfig)
        {
            m_managerCreator = managerCreator;
            m_buildDataConfig = buildDataConfig;
        }

        private readonly IManagerCreator m_managerCreator;
        private readonly BuildDataConfig m_buildDataConfig;
        private readonly string m_particleId = "firework";

        public async UniTask PlayFirework()
        {
            var particleSystems = new List<ParticleSystem>();
            var fireworkPositions = m_buildDataConfig.FireworkPositions;
            for (int i = 0; i < fireworkPositions.Count; i++)
            {
                var fireworkPosition = fireworkPositions[i];
                var particle = await m_managerCreator.Create<ParticleCreatedItem, ParticleCreator>(m_particleId);
                if (particle != null)
                {
                    particle.Particle.transform.position = fireworkPosition;
                    particle.Particle.Play();
                    particleSystems.Add(particle.Particle);
                }
            }
            await UniTask.WaitWhile(() => particleSystems.All(p => !p.gameObject.activeSelf) == false);
        }
    }
}