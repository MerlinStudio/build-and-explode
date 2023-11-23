using TMPro;
using UnityEngine;

namespace Game.View.Widgets
{
    public class AddBlockEffectWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_numberBlocks;

        public void SetNumberBlocks(int numberBlocks)
        {
            m_numberBlocks.text = $"+{numberBlocks}";
        }
    }
}