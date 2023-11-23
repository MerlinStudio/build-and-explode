using Game.Utils.Tools;
using UnityEngine;

namespace Game.View.Components
{
    public class MiniLevelComponent : MonoBehaviour
    {
        [SerializeField] private Bounds m_borderBounds;
        // Область добавленного объекта
        private Bounds m_newObjectBounds;
        // Добавленный 3д-объект
        private Transform m_targetTransform;
        // Cached tranform
        private Transform m_cachedTransform;
    
        public void SetupTransformObject(GameObject targetGameObject)
        {
            m_cachedTransform = transform;
            m_targetTransform = targetGameObject.transform;
            m_targetTransform.parent = m_cachedTransform;
            m_targetTransform.localScale = Vector3.one;
            m_targetTransform.localPosition = Vector3.zero;
            
            // Поиск границ объекта
            m_newObjectBounds = GameObjectTools.GetGameObjectBounds(targetGameObject);

            if (m_newObjectBounds.size.sqrMagnitude > 0)
            {
                // Масштабирование объекта
                Bounds globalBorderBounds = new Bounds(m_cachedTransform.TransformPoint(m_borderBounds.center), m_cachedTransform.TransformVector(m_borderBounds.size));
                float scaleFactor = TransformTools.Calculate3DObjectScale(m_newObjectBounds, globalBorderBounds);
                m_targetTransform = targetGameObject.transform;
                m_targetTransform.localScale *= scaleFactor;
                m_targetTransform.localPosition = m_cachedTransform.InverseTransformVector(
                    (globalBorderBounds.center - m_cachedTransform.position) - (m_newObjectBounds.center - m_cachedTransform.position) * scaleFactor);
            }
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.TransformPoint(m_borderBounds.center), transform.TransformVector(m_borderBounds.size));
        }
#endif
    }
}
