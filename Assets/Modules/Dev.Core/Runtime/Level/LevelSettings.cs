using System;
using Data.Builds.Configs;
using UnityEngine;

namespace Dev.Core.Level
{
    [Serializable]
    public class LevelSettings
    {
        [SerializeField] private BuildDataConfig m_buildDataConfig;

        public BuildDataConfig BuildDataConfig => m_buildDataConfig;
    }
}
