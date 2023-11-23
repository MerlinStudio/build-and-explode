using Dev.Core.Ui.UI.Manager;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class UnloadPanelCommand : PanelCommand
    {
        [Inject] private UiManager m_uiManager;

        public UnloadPanelCommand(UIPanel panel)
        {
            PanelId = panel.PanelId;
            Panel = panel;
        }

        protected override void Start()
        {
            //TEMP FIX???
            if (Panel == null)
            {
                OnCompleted(true);
                return;
            }
            
            Panel.DeInitialize();
        
            var panelOrder = Panel.GetPanelOrder();
            var layer = m_uiManager.GetOrCreateLayer(panelOrder);
            layer.RemovePanel(Panel);
        
            Object.Destroy(Panel.gameObject);
            m_uiManager.TryDestroyLayer(layer);
            
            OnCompleted(true);
        }
        
        public class Factory : PlaceholderFactory<UIPanel, UnloadPanelCommand>
        {
            
        }
    }
}