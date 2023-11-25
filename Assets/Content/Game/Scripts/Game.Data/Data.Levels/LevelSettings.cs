using System;
using Data.Builds.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Base.Dev.Core.Runtime.Level
{
    [Serializable]
    public class LevelSettings
    {
        [SerializeField] private BuildDataConfig m_buildDataConfig;
        [SerializeField] private MiniLevelData m_miniLevelData;

        public BuildDataConfig BuildDataConfig => m_buildDataConfig;
        public MiniLevelData MiniLevelData => m_miniLevelData;
    }

    [Serializable]
    public class MiniLevelData
    {
        public AssetReference AssetReference;
        [NonSerialized] public GameObject MiniLevelObject;
    }
}
