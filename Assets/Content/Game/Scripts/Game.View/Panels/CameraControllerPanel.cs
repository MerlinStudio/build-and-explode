using Dev.Core.Ui.UI.Panels;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.View.Panels
{
    public class CameraControllerPanel : UIPanel
    {
        [SerializeField] private Button m_activeRotateCameraButton;
        [SerializeField] private Image m_enableRotateCameraImage;
        [SerializeField] private Image m_disableRotateCameraImage;
        [SerializeField] private Transform m_parentImages;

        public ISubject<Unit> OnActiveRotateCamera;
            
        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            OnActiveRotateCamera = new Subject<Unit>();
            m_activeRotateCameraButton.onClick.AddListener(OnActiveRotateButtonClick);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            m_activeRotateCameraButton.onClick.RemoveListener(OnActiveRotateButtonClick);
            base.HidePanel(hideInstant);
        }

        public void SetInteractableButton(bool isInteractable)
        {
            m_activeRotateCameraButton.interactable = isInteractable;
        }

        public void SetActiveRotateButtonImage(bool isActive)
        {
            var duration = 0.2f;
            Sequence sequence = DOTween.Sequence().SetId(this);
            sequence.Append(m_parentImages.DORotate(Vector3.up * 90, duration))
                .AppendCallback(() =>
                {
                    m_enableRotateCameraImage.gameObject.SetActive(isActive);
                    m_disableRotateCameraImage.gameObject.SetActive(!isActive);
                });
            sequence.Insert(0, m_parentImages.DOScale(Vector3.one * 1.2f, duration));
            sequence.Append(m_parentImages.DORotate(Vector3.up * 180, duration));
            sequence.Insert(duration, m_parentImages.DOScale(Vector3.one, duration));
        }

        private void OnActiveRotateButtonClick()
        {
            OnActiveRotateCamera.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}