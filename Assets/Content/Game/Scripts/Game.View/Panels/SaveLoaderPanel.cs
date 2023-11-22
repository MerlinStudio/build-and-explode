using Dev.Core.Ui.UI.Panels;
using DG.Tweening;
using UnityEngine;

namespace Panels
{
    public class SaveLoaderPanel : UIPanel
    {
        [SerializeField] private Transform m_roundRectFill;

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            PlayLoadingAnimation();
        }

        public override void HidePanel(bool hideInstant = false)
        {
            OnDestroy();
            base.HidePanel(hideInstant);
        }

        private void PlayLoadingAnimation()
        {
            m_roundRectFill.DOLocalRotate(new Vector3(0,0, -360), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .SetId(this);
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}