using System;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.Extensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Widgets
{
    public class AnimationWidget : MonoBehaviour
    {
        [SerializeField] private AnimationData m_showAnimationData;
        [SerializeField] private AnimationData m_hideAnimationData;

        public async UniTask ShowUniTask()
        {
            switch (m_showAnimationData.AnimationType)
            {
                case EAnimationType.Move:
                    await MoveAnimation(m_showAnimationData);
                    break;
                case EAnimationType.Scale:
                    await ScaleAnimation(m_showAnimationData);
                    break;
                case EAnimationType.Alpha:
                    await AlphaAnimation(m_showAnimationData, 1);
                    break;
            }
        }

        public async void Show()
        {
            await ShowUniTask();
        }

        public async UniTask HideUniTask()
        {
            switch (m_hideAnimationData.AnimationType)
            {
                case EAnimationType.Move:
                    await MoveAnimation(m_hideAnimationData);
                    break;
                case EAnimationType.Scale:
                    await ScaleAnimation(m_hideAnimationData);
                    break;
                case EAnimationType.Alpha:
                    await AlphaAnimation(m_hideAnimationData, 0);
                    break;
            }
        }

        public async void Hide()
        {
            await HideUniTask();
        }

        private async UniTask MoveAnimation(AnimationData animationData)
        {
            if (!animationData.UseStartPositionAsCurrent)
            {
                transform.localPosition = animationData.StartPosition;
            }

            await transform.DOLocalMove(animationData.EndPosition, animationData.MoveDuration)
                .SetEase(animationData.MoveCurve).SetId(this);
        }
        
        private async UniTask ScaleAnimation(AnimationData animationData)
        {
            if (!animationData.UseStartScaleAsCurrent)
            {
                transform.localScale = animationData.StartScale;
            }
            
            await transform.DOScale(animationData.EndScale, animationData.ScaleDuration)
                .SetEase(animationData.ScaleCurve).SetId(this);
        }
        
        private async UniTask AlphaAnimation(AnimationData animationData, int alpha)
        {
            await animationData.CanvasGroup.DoAlpha(alpha, animationData.AlphaDuration).SetId(this);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }

    public enum EAnimationType
    {
        Move,
        Scale,
        Alpha
    }

    [Serializable]
    public class AnimationData
    {
        public EAnimationType AnimationType;

        [ShowIf("AnimationType", EAnimationType.Alpha)]
        public CanvasGroup CanvasGroup;
        [ShowIf("AnimationType", EAnimationType.Alpha)] [Min(0)]
        public float AlphaDuration;
        
        [ShowIf("AnimationType", EAnimationType.Move)]
        public bool UseStartPositionAsCurrent = true;
        [ShowIf("AnimationType", EAnimationType.Move), HideIf("UseStartPositionAsCurrent", false)]
        public Vector3 StartPosition;
        [ShowIf("AnimationType", EAnimationType.Move)]
        public Vector3 EndPosition;
        [ShowIf("AnimationType", EAnimationType.Move)] [Min(0)]
        public float MoveDuration;
        [ShowIf("AnimationType", EAnimationType.Move)]
        public AnimationCurve MoveCurve;
        
        [ShowIf("AnimationType", EAnimationType.Scale)]
        public bool UseStartScaleAsCurrent = true;
        [ShowIf("AnimationType", EAnimationType.Scale), HideIf("UseStartScaleAsCurrent", false)]
        public Vector3 StartScale;
        [ShowIf("AnimationType", EAnimationType.Scale)]
        public Vector3 EndScale;
        [ShowIf("AnimationType", EAnimationType.Scale)] [Min(0)]
        public float ScaleDuration;
        [ShowIf("AnimationType", EAnimationType.Scale)]
        public AnimationCurve ScaleCurve;
    }
}