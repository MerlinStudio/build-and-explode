using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class UpdatePanelCommand : PanelCommand
    {
        private readonly UIPanelData m_uiPanelData;

        public UpdatePanelCommand(UIPanel uiPanel, UIPanelData uiPanelData)
        {
            PanelId = uiPanel.PanelId;
            Panel = uiPanel;
            m_uiPanelData = uiPanelData;
        }

        protected override void Start()
        {
            Panel.UpdatePanel(m_uiPanelData);
            OnCompleted(true);
        }

        public class Factory : PlaceholderFactory<UIPanel, UIPanelData, UpdatePanelCommand>
        {
        }
    }
}