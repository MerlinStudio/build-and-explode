using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Dev.Core.Ui.Extensions
{ 
    public static class CanvasGroupTween
    {
        public static TweenerCore<float, float, FloatOptions> DoAlpha(
            this CanvasGroup target,
            float endValue,
            float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(
                () => target.alpha,
                x => target.alpha = x,
                endValue, duration);
            return t;
        }
    }
}