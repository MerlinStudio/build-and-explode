using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(LevelMapConfig), fileName = nameof(LevelMapConfig))]

    public class LevelMapConfig : ScriptableObject
    {
        [SerializeField] private AssetReference m_levelMapComponentReference
;
        public AssetReference LevelMapComponentReference => m_levelMapComponentReference;
    }
}