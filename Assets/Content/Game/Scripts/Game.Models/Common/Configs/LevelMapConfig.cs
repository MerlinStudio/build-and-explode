using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Common.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(LevelMapConfig), fileName = nameof(LevelMapConfig))]

    public class LevelMapConfig : ScriptableObject
    {
        [SerializeField] private AssetReference m_levelMapComponentReference;
        [SerializeField] private AssetReference m_lockedMiniMapReference;
        public AssetReference LevelMapComponentReference => m_levelMapComponentReference;
        public AssetReference LockedMiniMapReference => m_lockedMiniMapReference;
    }
}