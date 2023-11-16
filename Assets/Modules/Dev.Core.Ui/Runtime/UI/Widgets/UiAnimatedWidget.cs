using System;
using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using UniRx;
using UnityEngine;

namespace Dev.Core.Ui.UI.Widgets
{
    public class UiAnimatedWidget : MonoBehaviour
    {
        [SerializeField] private UIPanelAnimator m_uiPanelAnimator;

        private BoolReactiveProperty m_isShowed = new BoolReactiveProperty(false);

        /// <summary>
        /// Null if not require PanelData
        /// </summary>
        public virtual Type RequiredPanelDataType => null;
        protected UIPanelData BaseData { get; private set; }

        public IReadOnlyReactiveProperty<bool> IsShowed => m_isShowed;
        public UIPanelAnimator UIPanelAnimator => m_uiPanelAnimator;


        public virtual void ShowWidget(UIPanelData widgetData = null, bool immediate = false)
        {
            m_isShowed.Value = true;
            BaseData = widgetData;
            m_uiPanelAnimator.PlayShow(immediate);
        }
        public virtual void HideWidget(bool immediate = false)
        {
            m_uiPanelAnimator.PlayHide(immediate);
            BaseData = null;
            m_isShowed.Value = false;
        }
    }
}