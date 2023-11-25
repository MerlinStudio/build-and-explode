using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Base.Dev.Core.Runtime.Level
{
    [Serializable]
    public class LevelData
    {
        [field: SerializeField]
        public bool IsRepeat { get; private set; }
        
        [field: SerializeField]
        public string LevelID { get; set; }
        
        [field: SerializeField]
        public bool UseCustomLevelInfo { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(UseCustomLevelInfo))]
        public AssetReference LevelReference { get; private set; }

        [field: SerializeField]
        public LevelSettings LevelSettings { get; set; }
    }
}