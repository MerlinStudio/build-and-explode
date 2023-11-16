using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class ChangePanelCommand : PanelCommand
    {
        private UIPanel m_hidingPanel;
        private string m_showingPanelGuid;
        private readonly Camera m_camera;
        private readonly UIPanelData m_data;
        private readonly DiContainer m_injectContainer;
        private readonly bool m_isInstanceAnim;

        [Inject] private ShowPanelCommand.Factory m_showPanelCommandFactory;
        [Inject] private HidePanelCommand.Factory m_hidePanelCommandFactory;

        public ChangePanelCommand(UIPanel panel, string showingPanelGuid, Camera camera, UIPanelData data,
            DiContainer injectContainer, bool isInstanceAnimAnim)
        {
            PanelId = panel.PanelId;
            m_hidingPanel = panel;
            m_showingPanelGuid = showingPanelGuid;
            m_camera = camera;
            m_data = data;
            m_injectContainer = injectContainer;
            m_isInstanceAnim = isInstanceAnimAnim;
        }

        protected override void Start()
        {
            var showPanelCommand = m_showPanelCommandFactory.Create(m_showingPanelGuid, m_camera, m_data,
                m_injectContainer, m_isInstanceAnim);
            showPanelCommand.PanelLoadedEvent += OnNewPanelLoaded;
            showPanelCommand.Completed += OnShowPanelCompleted;
            showPanelCommand.Execute();
        }

        private void OnShowPanelCompleted(ICommand command, bool result)
        {
            Panel = ((ShowPanelCommand) command).Panel;
            OnCompleted(result);
        }

        private void OnNewPanelLoaded()
        {
            var hidePanelCommand = m_hidePanelCommandFactory.Create(m_hidingPanel, m_isInstanceAnim);
            hidePanelCommand.Execute();
        }

        public class Factory : PlaceholderFactory<UIPanel, string, Camera, UIPanelData, DiContainer, bool,
            ChangePanelCommand>
        {
        }
    }
}