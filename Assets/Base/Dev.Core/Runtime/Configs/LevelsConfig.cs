using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Core.Level
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