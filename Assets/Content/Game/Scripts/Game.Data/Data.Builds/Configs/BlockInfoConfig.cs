using Data.Builds.Blocks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Data.Builds.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(BlockInfoConfig), fileName = nameof(BlockInfoConfig))]

    public class BlockInfoConfig : ScriptableObject
    {
        [SerializeField, ReadOnly] private string m_id;
        [SerializeField] private AssetReference m_blockReference;
        [SerializeField] private BlockPropertyInfo m_blockPropertyInfo;

        public string Id => m_id;

        public AssetReference BlockReference => m_blockReference;
        public BlockPropertyInfo BlockPropertyInfo => m_blockPropertyInfo;

        [OnInspectorGUI]
        private void UpdateBlockId()
        {
            m_id = m_blockPropertyInfo.Id;
        }
    }
}