using System;
using System.Collections.Generic;
using Data.Explosion.Info;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(EnvironmentInfoConfig), fileName = nameof(EnvironmentInfoConfig))]

    public class EnvironmentInfoConfig : ScriptableObject
    {
        [SerializeField] private EnvironmentInfo m_environmentInfo;
        [SerializeField] private BuildAnimationInfo m_buildAnimationInfo;
        [SerializeField] private ClickEffectAnimationInfo m_clickEffectAnimationInfo;
        [SerializeField] private List<ParticleSystemInfo> m_particlesInfo;
        [SerializeField] private AssetReference m_addBlockEffectWidgetReference;

        public EnvironmentInfo EnvironmentInfo => m_environmentInfo;
        public BuildAnimationInfo BuildAnimationInfo => m_buildAnimationInfo;
        public ClickEffectAnimationInfo ClickEffectAnimationInfo => m_clickEffectAnimationInfo;
        public List<ParticleSystemInfo> ParticlesInfo => m_particlesInfo;
        public AssetReference AddBlockEffectWidgetReference => m_addBlockEffectWidgetReference;
    }
    
    [Serializable]
    public class BuildAnimationInfo
    {
        public float ScaleDuration = 0.15f;
        public AnimationCurve ScaleCurve;
        public float FallHeight = 5;
        public float MoveDuration = 0.25f;
        public AnimationCurve MoveCurve;
    }
    
    [Serializable]
    public class ClickEffectAnimationInfo
    {
        public float FlyHeight = 200;
        public float MoveDuration = 0.5f;
        public AnimationCurve MoveCurve;
    }
    
    [Serializable]
    public class ParticleSystemInfo
    {
        public string Id;
        public AssetReference ParticleReference;
    }
}