using Dev.Core.Ui.UI.Panels;
using UniRx;

namespace Dev.Core.Ui.Commands
{
    public abstract class PanelCommand : AbstractCommand
    {
        public UIPanel Panel { get; protected set; }
        public string PanelId { get; protected set; }

        protected bool Canceled;
        private bool m_started;
        
        public virtual void Cancel()
        {
            Canceled = true;
            OnCompleted(false);
        }

        public override void Execute()
        {
            Observable.NextFrame().Subscribe(unit =>
            {
                if (Canceled || m_started) return;
                m_started = true;
                Start();
            });
        }

        protected abstract void Start();
    }
}