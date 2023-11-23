using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.View.Widgets
{
    public class CountdownExplosionWidget : MonoBehaviour
    {
        [SerializeField] private List<Transform> m_countdownDigits;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public async UniTask PlayCountdownExplosion()
        {
            var sequence = DOTween.Sequence();
            for (int i = 0; i < m_countdownDigits.Count; i++)
            {
                var countdownText = m_countdownDigits[i];
                PlayAnimation(sequence, countdownText);
            }
            await sequence;
        } 

        private void PlayAnimation(Sequence sequence, Transform countdownText)
        {
            countdownText.localScale = Vector3.zero;
            sequence.Append(countdownText.DOScale(Vector3.one, 0.7f)
                .OnComplete(() =>
                {
                    countdownText.gameObject.SetActive(false);
                })
                .SetEase(Ease.InQuad)
                .SetId(this));
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}