using System;
using Dev.Core.Ui.UI.Panels;
using Lean.Touch;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Panels
{
    public class BuildClickerPanel : UIPanel
    {
        [SerializeField] private Button m_buildClickerButton;
        [SerializeField] private Transform m_effectParent;

        public Transform EffectParent => m_effectParent;
        public ISubject<Unit> OnButtonClicked { get; private set; }

        private RectTransform m_canvasRectTransform;
        private Vector3 m_screenPosition;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            OnButtonClicked = new Subject<Unit>();
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
            OnButtonClicked.OnNext(Unit.Default);
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