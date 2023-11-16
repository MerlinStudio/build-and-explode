using Dev.Core.Ui.UI.Panels;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Core.PanelExample
{
    public class ResultPanel : UIPanel
    {
        [SerializeField]
        private Button exitButton;
        
        [SerializeField]
        private Button continueButton;
        
        public ISubject<Unit> EventExitLevel;
        public ISubject<Unit> EventContinueLevel;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            EventExitLevel = new Subject<Unit>();
            EventContinueLevel = new Subject<Unit>();
            exitButton.onClick.AddListener(OnExitButtonClick);
            continueButton.onClick.AddListener(OnContinueButtonClick);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClick);
            continueButton.onClick.RemoveListener(OnContinueButtonClick);
            base.HidePanel(hideInstant);
        }
        
        private void OnExitButtonClick()
        {
            EventExitLevel.OnNext(Unit.Default);
        }
        
        private void OnContinueButtonClick()
        {
            EventContinueLevel.OnNext(Unit.Default);
        }
    }
}