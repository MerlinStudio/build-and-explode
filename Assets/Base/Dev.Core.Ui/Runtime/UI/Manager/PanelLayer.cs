using System.Collections.Generic;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Core.Ui.UI.Manager
{
    public class PanelLayer : MonoBehaviour
    {
        [SerializeField] private Canvas m_canvas = null;
        [SerializeField] private CanvasScaler m_canvasScaler = null;
    
        private List<UIPanel> m_panels = new List<UIPanel>();

        public int LayerOrder => m_canvas.sortingOrder;
        public bool HasPanel => m_panels.Count > 0;
    
        public CanvasScaler CanvasScaler => m_canvasScaler;
        public Canvas Canvas => m_canvas;

        public void SetLayer(int order)
        {
            gameObject.name = $"{name}_{order}";
            m_canvas.sortingOrder = order;
            gameObject.SetActive(true);
        }

        public void AddPanel(UIPanel panel)
        {
            m_panels.Add(panel);
        }

        public void RemovePanel(UIPanel panel)
        {
            m_panels.Remove(panel);
        }
    }
}