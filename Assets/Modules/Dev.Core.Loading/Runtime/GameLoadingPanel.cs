using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;

namespace Dev.Core.Loading
{
    public class GameLoadingPanel : UIPanel
    {
        [SerializeField]
        private ProgressBar progressBar;

        public override void Initialize(UIPanelData data = null)
        {
            base.Initialize(data);
            progressBar.Initialization();
            progressBar.SetNormalizedProgress(0, true);
        }

        public void SetProgress(float progressValue, bool instant = false)
        {
            progressBar.SetNormalizedProgress(progressValue, instant);
        }
    }
}
