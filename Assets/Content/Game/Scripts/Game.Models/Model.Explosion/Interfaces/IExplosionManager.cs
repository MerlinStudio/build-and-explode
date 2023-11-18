using Data.Explosion.Configs;
using Model.Creator.Interfaces;
using UnityEngine;

namespace Model.Explosion.Interfaces
{
    public interface IExplosionManager
    {
        void Init(IBuildProvider buildProvider, IBlockCreator BlockCreator, EnvironmentInfoConfig environmentInfoConfig);
        void DeInit();
        void AddBombInfo(int radius, int force, float delay);
        void Explosion();
        void ChangeBuildingLayer(float value);
    }
}