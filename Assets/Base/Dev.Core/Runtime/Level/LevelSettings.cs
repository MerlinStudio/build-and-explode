using System;
using Data.Builds.Configs;
using Game.View.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Dev.Core.Level
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
