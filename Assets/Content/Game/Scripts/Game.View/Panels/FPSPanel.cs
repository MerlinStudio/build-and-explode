using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.View.Panels
{
    public class FPSPanel : UIPanel
    {
        [SerializeField] private Button m_button;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_button.onClick.AddListener(OnResetSaveData);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_button.onClick.RemoveListener(OnResetSaveData);
            base.HidePanel(hideInstant);
        }

        private void OnResetSaveData()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
        }
    }
}
