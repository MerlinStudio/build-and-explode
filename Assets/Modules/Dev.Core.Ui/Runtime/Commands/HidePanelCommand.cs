using System;
using Dev.Core.Ui.UI.Manager;
using Dev.Core.Ui.UI.Panels;
using UniRx;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class HidePanelCommand : PanelCommand
    {
        private readonly bool m_isInstanceAnim;
        [Inject] private UiManager m_uiManager;
        [Inject] private UnloadPanelCommand.Factory m_unloadPanelCommandFactory;

        private IDisposable m_disposable;

        public HidePanelCommand(UIPanel panel, bool isInstanceAnim)
        {
            PanelId = panel.PanelId;
            Panel = panel;
            m_isInstanceAnim = isInstanceAnim;
        }

        protected override void Start()
        {
            m_disposable = ObservableExtensions.Subscribe<long>(Observable.EveryUpdate(), CheckPanelState);
            Panel.HidePanel(m_isInstanceAnim);
        }

        public override void Cancel()
        {
            base.Cancel();
            m_uiManager.AddPanel(Panel);
        }

        private void CheckPanelState(long l)
        {
            if (Panel.State == UIPanel.PanelState.Hide)
            {
                m_disposable.Dispose();
                var unloadPanelCommand = m_unloadPanelCommandFactory.Create(Panel);
                unloadPanelCommand.Completed += OnUnload;
                unloadPanelCommand.Execute();
            }
        }

        private void OnUnload(ICommand command, bool result)
        {
            OnCompleted(result);
        }

        public class Factory : PlaceholderFactory<UIPanel, bool, HidePanelCommand>
        {
        }
    }
}