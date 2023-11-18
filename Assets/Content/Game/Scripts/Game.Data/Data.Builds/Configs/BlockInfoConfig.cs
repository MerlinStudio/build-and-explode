using Data.Builds.Blocks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Data.Builds.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(BlockInfoConfig), fileName = nameof(BlockInfoConfig))]

    public class BlockInfoConfig : ScriptableObject
    {
        [SerializeField] private string m_id;
        [SerializeField] private AssetReference m_blockReference;
        [SerializeField] private BlockPropertyInfo m_blockPropertyInfo;

        public string Id => m_id;
        public AssetReference BlockReference => m_blockReference;
        public BlockPropertyInfo BlockPropertyInfo => m_blockPropertyInfo;
    }
}