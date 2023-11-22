using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Creators;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using State.Creator.Interfaces;
using UnityEngine;
using Widgets;

namespace State.Creator.Controllers
{
    public class ClickEffectController
    {
        public ClickEffectController(
            IManagerCreator managerCreator,
            ClickEffectAnimationInfo clickEffectAnimationInfo)
        {
            m_managerCreator = managerCreator;
            m_clickEffectAnimationInfo = clickEffectAnimationInfo;
        }

        private readonly IManagerCreator m_managerCreator;
        private readonly ClickEffectAnimationInfo m_clickEffectAnimationInfo;

        private int m_amountBlock;
        private List<AddBlockEffectWidget> m_addBlockEffectPool;

        public bool IsAllAnimationFinished => m_addBlockEffectPool.All(e => !e.gameObject.activeSelf);

        public void Init()
        {
            m_addBlockEffectPool = new List<AddBlockEffectWidget>();
        }
        
        public void DeInit()
        {
            m_addBlockEffectPool.Clear();
        }

        public async void PlayAddBlockEffect(Vector2 position)
        {
            var addBlockEffectWidget = await PopEffect();
            SetInfo(addBlockEffectWidget);
            PlayAnimation(addBlockEffectWidget, position);
        }

        public void UpdateAmountBlock(int amountBlock)
        {
            m_amountBlock = amountBlock;
        }

        private void SetInfo(AddBlockEffectWidget addBlockEffectWidget)
        {
            addBlockEffectWidget.SetNumberBlocks(m_amountBlock);
        }

        private void PlayAnimation(AddBlockEffectWidget addBlockEffectWidget, Vector2 position)
        {
            var rectTransform = addBlockEffectWidget.transform as RectTransform;
            rectTransform.anchoredPosition = position;
            var endPosition = position + Vector2.up * m_clickEffectAnimationInfo.FlyHeight;
            var sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOLocalMove(endPosition, m_clickEffectAnimationInfo.MoveDuration))
                .SetEase(m_clickEffectAnimationInfo.MoveCurve)
                .OnComplete(() =>
                {
                    PushEffect(addBlockEffectWidget);
                });
        }

        private async UniTask<AddBlockEffectWidget> PopEffect()
        {
            if (m_addBlockEffectPool.Count > 0)
            {
                var addBlockEffectWidget = m_addBlockEffectPool.Find(e => !e.gameObject.activeSelf);
                if (addBlockEffectWidget != null)
                {
                    addBlockEffectWidget.gameObject.SetActive(true);
                    return addBlockEffectWidget;
                }
            }
            var addBlockEffect = await m_managerCreator.Create<AddBlockEffect, UiEffectCreator>(String.Empty);
            m_addBlockEffectPool.Add(addBlockEffect.AddBlockEffectWidget);
            addBlockEffect.AddBlockEffectWidget.gameObject.SetActive(true);
            return addBlockEffect.AddBlockEffectWidget;
        }

        private void PushEffect(AddBlockEffectWidget addBlockEffectWidget)
        {
            if (m_addBlockEffectPool.Contains(addBlockEffectWidget))
            {
                addBlockEffectWidget.gameObject.SetActive(false);
                return;
            }
            
            addBlockEffectWidget.gameObject.SetActive(false);
            m_addBlockEffectPool.Add(addBlockEffectWidget);
        }
    }
}