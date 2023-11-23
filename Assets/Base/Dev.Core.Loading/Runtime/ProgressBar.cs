using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Core.Loading
{
    public class ProgressBar : MonoBehaviour
    {
        public enum FillType
        {
            FillAmount = 2,
            Torsion = 3
        }
        
        [SerializeField, ShowIf("fillType", FillType.FillAmount)]
        private Image imageFillAmount;
        
        [SerializeField, ShowIf("fillType", FillType.Torsion)]
        private Transform roundRectFill;
        
        [SerializeField]
        private FillType fillType = FillType.FillAmount;

        [SerializeField]
        private float fillDuration = 0.25f;
        
        [SerializeField]
        private Ease fillEase = Ease.OutCirc;

        private Sequence sequence;

        private void OnDestroy()
        {
            DOTween.Kill(this);
            sequence.Kill();
        }

        public virtual void Initialization()
        {
            roundRectFill.gameObject.SetActive(fillType == FillType.Torsion);
            imageFillAmount.gameObject.SetActive(fillType == FillType.FillAmount);
        }

        public virtual void SetNormalizedProgress(float progress, bool instant = false)
        {
            switch (fillType)
            {
                case FillType.FillAmount:
                    FillAmountProgress(progress, instant);
                    return;
                case FillType.Torsion:
                    PlayLoadingAnimation();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected virtual void FillAmountProgress(float targetValue, bool instant = false)
        {
            DOTween.Kill(this);

            if (instant)
            {
                imageFillAmount.fillAmount = targetValue;
            }
            else
            {
                imageFillAmount.DOFillAmount(targetValue, fillDuration).SetEase(fillEase).SetId(this);
            }
        }

        public void PlayLoadingAnimation()
        {
            sequence = DOTween.Sequence();
            sequence.Append(roundRectFill.DOLocalRotate(new Vector3(0,0, -360), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        }
    }
}