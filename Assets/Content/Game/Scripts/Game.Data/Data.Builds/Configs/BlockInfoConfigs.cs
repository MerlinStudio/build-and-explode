using System.Collections.Generic;
using UnityEngine;

namespace Data.Builds.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(BlockInfoConfigs), fileName = nameof(BlockInfoConfigs))]

    public class BlockInfoConfigs : ScriptableObject
    {
        [SerializeField] private List<BlockInfoConfig> m_blockInfoList;

        public List<BlockInfoConfig> BlockInfoList => m_blockInfoList;
    }
}