using System.Collections.Generic;
using UnityEngine;

namespace Data.Explosion.Configs
{
    [CreateAssetMenu(menuName = "Configs" + "/" + nameof(BuildDataConfig), fileName = nameof(BuildDataConfig))]
    public class BuildDataConfig : ScriptableObject
    {
        [SerializeField] private List<Vector3> m_buildData;

        public List<Vector3> BuildData
        {
            get => m_buildData;
            set => m_buildData = value;
        }
    }
}
