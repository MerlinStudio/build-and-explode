using TMPro;
using UnityEngine;

namespace Game.View.Components
{
    public class LevelMapComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_numberBlocksText;
        [SerializeField] private MiniLevelComponent m_miniLevelComponent;
        
        public void SetMiniLevelObject(GameObject miniLevelObject)
        {
            m_miniLevelComponent.SetupTransformObject(miniLevelObject);
        }

        public void SetNumberBlocks(int numberBlocks)
        {
            var numberBlocksText = numberBlocks > 0 ? $"{numberBlocks}" : "???";
            m_numberBlocksText.text = numberBlocksText;
        }
    }
}