using TMPro;
using UnityEngine;

namespace Game.View.Components
{
    public class LevelMapComponent : MonoBehaviour
    {
        [SerializeField] private Transform m_miniLevelParent;
        [SerializeField] private TextMeshProUGUI m_numberBlocksText;
        [SerializeField] private MiniLevelComponent m_miniLevelComponent;
        
        public void SetMiniLevelObject(GameObject miniLevelObject)
        {
            //miniLevelObject.transform.SetParent(m_miniLevelParent);
            //miniLevelObject.transform.localPosition = Vector3.zero;
            m_miniLevelComponent.SetupTransformObject(miniLevelObject);
        }

        public void SetNumberBlocks(int numberBlocks)
        {
            m_numberBlocksText.text = $"{numberBlocks}";
        }


    }
}