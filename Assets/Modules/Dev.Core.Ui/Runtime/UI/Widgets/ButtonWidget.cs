using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dev.Core.Ui.UI.Widgets
{
    public class ButtonWidget : UiAnimatedWidget
    {
        [SerializeField] protected Button m_button;
        [SerializeField] protected Animator m_targetAnimator;
        [SerializeField] protected string m_clickTrigger = "click";

        public Button Button => m_button;

        public void AddListener(UnityAction action)
        {
            if (m_button == null) return;
            m_button.onClick.AddListener(action);
        }

        public void RemoveListener(UnityAction action)
        {
            if (m_button == null) return;
            m_button.onClick.RemoveListener(action);
        }

        protected virtual void Awake()
        {
            if (m_button == null) return;
            m_button.onClick.AddListener(OnClick);
        }

        protected virtual void OnDestroy()
        {
            if (m_button == null) return;
            m_button.onClick.RemoveListener(OnClick);
        }

        protected virtual void OnClick()
        {
            if (m_targetAnimator == null) return;
            m_targetAnimator.SetTrigger(m_clickTrigger);
        }

        public override void ShowWidget(UIPanelData widgetData = null, bool immediate = false)
        {
            base.ShowWidget(widgetData, immediate);
            SetInteractable(true);
        }

        public override void HideWidget(bool immediate = false)
        {
            SetInteractable(false);
            base.HideWidget(immediate);
        }

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetInteractable(bool interactable)
        {
            if (m_button == null) return;
            m_button.interactable = interactable;
        }
    }
}