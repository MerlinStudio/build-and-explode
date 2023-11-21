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
        [SerializeField] private List<Vector3> m_fireworkPositions;
        [SerializeField] private int m_amountBomb;

        public List<BlockData> BlockData
        {
            get => m_blockData;
            set => m_blockData = value;
        }
        
        public List<Vector3> FireworkPositions
        {
            get => m_fireworkPositions;
            set => m_fireworkPositions = value;
        }
        
        public int AmountBomb
        {
            get => m_amountBomb;
            set => m_amountBomb = value;
        }
    }

    [Serializable]
    public class BlockData
    {
        [ReadOnly] public string Id;
        public Vector3Int Coord;
    }
}
