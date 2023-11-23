using Dev.Core.Ui.UI.Widgets;
using DG.Tweening;
using UnityEngine;

namespace Dev.Core.Ui.UI.Panels
{
    public class ExamplePanel : UIPanel
    {
        [SerializeField] private UiAnimatedWidget m_animatedWidget;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);

            m_animatedWidget.ShowWidget();
            m_animatedWidget.transform.DOLocalRotate(
                new Vector3(0, 0, -360), 2f, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_animatedWidget.HideWidget();
            
            base.HidePanel(hideInstant);
        }
    }
}