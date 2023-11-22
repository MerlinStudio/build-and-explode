using System.Collections.Generic;
using Configs;
using Creators;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using State.Creator.Interfaces;
using UnityEngine;

namespace State.Creator.Controllers
{
    public class BlockAnimationController
    {
        public BlockAnimationController(BuildAnimationInfo animationInfo, IManagerCreator managerCreator)
        {
            m_animationInfo = animationInfo;
            m_managerCreator = managerCreator;
        }
        
        private readonly BuildAnimationInfo m_animationInfo;
        private readonly IManagerCreator m_managerCreator;
        private readonly string m_particleId = "end move block";

        private Queue<Transform> m_blocksQueue;
        private bool m_isStartAnimation;

        public bool IsAllAnimationFinished => m_blocksQueue.Count <= 0;

        public void Init()
        {
            m_blocksQueue = new Queue<Transform>();
        }

        public void DeInit()
        {
            m_blocksQueue.Clear();
        }

        public void PlayAnimation(List<Transform> blockTransforms)
        {
            for (int i = 0; i < blockTransforms.Count; i++)
            {
                var blocTransform = blockTransforms[i];
                blocTransform.gameObject.SetActive(false);
                m_blocksQueue.Enqueue(blocTransform);
            }

            if (!m_isStartAnimation)
            {
                StartAnimation();
            }
        }

        private async void StartAnimation()
        {
            m_isStartAnimation = true;

            while(m_blocksQueue.Count > 0)
            {
                await UniTask.Delay(100);
                
                var block = m_blocksQueue.Dequeue();
                block.gameObject.SetActive(true);
                var sequence = DOTween.Sequence();
                sequence = PlayScaleAnimation(sequence, block);
                sequence = PlayMoveAnimation(sequence, block);
                EndMoveAnimation(sequence, block);
            }
            
            m_isStartAnimation = false;
        }

        private Sequence PlayScaleAnimation(Sequence sequence, Transform block)
        {
            var endScale = block.localScale;
            block.localScale = Vector3.one * 0.1f;
            sequence.Append(block.DOScale(endScale, m_animationInfo.ScaleDuration)
                .SetEase(m_animationInfo.ScaleCurve));
            return sequence;
        }
        
        private Sequence PlayMoveAnimation(Sequence sequence, Transform block)
        {
            var endPosition = block.position;
            var startPosition = Vector3.up * m_animationInfo.FallHeight;
            block.position = endPosition + startPosition;
            sequence.Append(block.DOMove(endPosition, m_animationInfo.MoveDuration)
                .SetEase(m_animationInfo.MoveCurve));
            return sequence;
        }

        private void EndMoveAnimation(Sequence sequence, Transform block)
        {
            sequence.OnComplete(() =>
            {
                PlayParticle(block);
            });
        }

        private async void PlayParticle(Transform block)
        {
            var particle = await m_managerCreator.Create<ParticleCreatedItem, ParticleCreator>(m_particleId);
            if (particle != null)
            {
                particle.Particle.transform.position = block.position + (Vector3.down * 0.5f);
                particle.Particle.Play();
            }
        }
    }
}