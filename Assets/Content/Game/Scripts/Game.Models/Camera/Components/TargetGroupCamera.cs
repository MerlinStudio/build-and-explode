using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Models.Camera.Components
{
    public class TargetGroupCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineTargetGroup m_cinemachineTargetGroup;
        [SerializeField] private Transform m_targetReference;
        [SerializeField] private Transform m_targetsParent;
        [SerializeField] private Transform m_rotateParent;
        [SerializeField] private float rotationSpeed;

        private Transform m_targetGroupTopTransform;
        private Sequence m_rotatorSequence;
        private bool m_isRotate;

        public void Init(List<Vector3> botPositions, Vector3 topPosition)
        {
            for (int i = 0; i < botPositions.Count; i++)
            {
                var position = botPositions[i];
                var targetTransform = Instantiate(m_targetReference, m_targetsParent);
                targetTransform.position = position;
                m_cinemachineTargetGroup.AddMember(targetTransform, 0.5f, 1);
            }

            m_targetGroupTopTransform = Instantiate(m_targetReference, m_targetsParent);
            m_targetGroupTopTransform.position = topPosition;
            m_cinemachineTargetGroup.AddMember(m_targetGroupTopTransform, 1, 1);
        }

        private void Update()
        {
            if (!m_isRotate)
            {
                return;
            }
            m_rotateParent.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
        
        public void UpdateTargetGroupTopHeight(float height)
        {
            m_targetGroupTopTransform.DOMoveY(height, 0.5f).SetId(this);
        }

        public void ActiveRotator(bool isActive)
        {
            m_isRotate = isActive;
        }

        [Button]
        private void Active()
        {
            ActiveRotator(true);
        }

        [Button]
        private void DeActive()
        {
            ActiveRotator(false);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
        }
    }
}