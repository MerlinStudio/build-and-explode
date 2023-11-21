using System;
using Dev.Core.Ui.UI.Panels;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

namespace Panels
{
    public class BuildClickerPanel : UIPanel
    {
        public event Action EventButtonClicked;

        [SerializeField] private Button m_buildClickerButton;
        [SerializeField] private Transform m_effectParent;

        public Transform EffectParent => m_effectParent;
        private RectTransform m_canvasRectTransform;
        
        private Vector3 m_screenPosition;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            m_buildClickerButton.onClick.AddListener(OnButtonClick);
            m_canvasRectTransform = Canvas.transform as RectTransform;
            LeanTouch.OnFingerDown += OnFingerDown;
        }

        public override void HidePanel(bool hideInstant = false)
        {
            LeanTouch.OnFingerDown -= OnFingerDown;
            m_buildClickerButton.onClick.RemoveListener(OnButtonClick);
            base.HidePanel(hideInstant);
        }

        private void OnButtonClick()
        {
            EventButtonClicked?.Invoke();
        }
        
        private void OnFingerDown(LeanFinger leanFinger)
        {
            m_screenPosition = leanFinger.ScreenPosition;
        }
        
        public Vector2 GetAddBlockEffectPosition()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvasRectTransform, m_screenPosition, UiCamera, out var localPos);
            return localPos;
        }
    }
}