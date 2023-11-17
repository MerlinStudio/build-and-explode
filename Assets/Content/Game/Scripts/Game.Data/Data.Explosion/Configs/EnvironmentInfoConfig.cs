using Data.Explosion.Info;
using UnityEngine;

namespace Data.Explosion.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(EnvironmentInfoConfig), fileName = nameof(EnvironmentInfoConfig))]

    public class EnvironmentInfoConfig : ScriptableObject
    {
        [SerializeField] private EnvironmentInfo m_environmentInfo;

        public EnvironmentInfo EnvironmentInfo => m_environmentInfo;
    }
}