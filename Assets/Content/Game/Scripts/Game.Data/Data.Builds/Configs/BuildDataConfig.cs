using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.Builds.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(BuildDataConfig), fileName = nameof(BuildDataConfig))]
    public class BuildDataConfig : ScriptableObject
    {
        [SerializeField] private List<BlockData> m_blockData;

        public List<BlockData> BlockData
        {
            get => m_blockData;
            set => m_blockData = value;
        }
    }

    [Serializable]
    public class BlockData
    {
        [ReadOnly] public string Id;
        public Vector3Int Coord;
    }
}
