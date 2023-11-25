using System;
using System.Collections.Generic;
using Base.Dev.Core.Runtime.Level;
using UnityEngine;

namespace Base.Dev.Core.Runtime.Configs
{
    [CreateAssetMenu(fileName = nameof(LevelsConfig), menuName = "Configs/GameCore/"+nameof(LevelsConfig), order = 0)]
    public class LevelsConfig : ScriptableObject
    {
        [SerializeField] private List<LevelPack> levelPack;
        
        public List<LevelPack> LevelPacks => levelPack;
    }

    [Serializable]
    public class LevelPack
    {
        public int PackId;
        public List<LevelData> LevelsData;
    }
}