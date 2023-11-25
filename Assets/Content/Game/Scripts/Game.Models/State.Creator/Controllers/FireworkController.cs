using System.Collections.Generic;
using System.Linq;
using Common.Creators;
using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using State.Creator.Interfaces;
using State.LevelLoader.Interfaces;
using UnityEngine;

namespace State.Creator.Controllers
{
    public class FireworkController
    {
        public FireworkController(
            IManagerCreator managerCreator,
            ILevelProvider levelProvider)
        {
            m_managerCreator = managerCreator;
            m_levelProvider = levelProvider;
        }

        private readonly IManagerCreator m_managerCreator;
        private readonly ILevelProvider m_levelProvider;
        private readonly string m_particleId = "firework";

        public async UniTask PlayFirework()
        {
            var particleSystems = new List<ParticleSystem>();
            var buildDataConfig = m_levelProvider.GetCurrentBuildDataConfig();
            var fireworkPositions = buildDataConfig.FireworkPositions;
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