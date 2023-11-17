using Data.Explosion.Configs;
using Model.Creator.Interfaces;
using UnityEngine;

namespace Model.Explosion.Interfaces
{
    public interface IExplosionManager
    {
        void Init(IBuildProvider buildProvider, IBlockCreator BlockCreator, EnvironmentInfoConfig environmentInfoConfig);
        void DeInit();
        void AddBomb(Transform blockTransform, int radius, int force, float delay);
        void Explosion();
    }
}