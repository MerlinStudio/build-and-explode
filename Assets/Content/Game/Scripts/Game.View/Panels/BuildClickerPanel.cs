using System;
using Dev.Core.Ui.UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class BuildClickerPanel : UIPanel
    {
        public event Action EventButtonClicked;

        [SerializeField] private Button m_buildClickerButton;
        
        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_buildClickerButton.onClick.AddListener(OnButtonClick);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_buildClickerButton.onClick.RemoveListener(OnButtonClick);
            base.HidePanel(hideInstant);
        }

        private void OnButtonClick()
        {
            EventButtonClicked?.Invoke();
        }
    }
}