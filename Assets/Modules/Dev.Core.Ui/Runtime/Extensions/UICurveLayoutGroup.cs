using UnityEngine;
using UnityEngine.Assertions;

namespace Dev.Core.Ui.Extensions
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class UICurveLayoutGroup : MonoBehaviour
    {
        // FIELDS: serialize
        [SerializeField] private AnimationCurve m_animationCurve;
        [SerializeField] private int m_curveStepsCount = 50;


        // FIELDS: private
        private RectTransform m_cachedRectTransform;


        // METHODS
        protected void Awake()
        {
            m_cachedRectTransform = (RectTransform)transform;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            { 
                m_cachedRectTransform = (RectTransform)transform;

                //
                UpdateLayout();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("UpdateLayout()")]
        public void UpdateLayout()
        {
            Assert.IsNotNull(m_cachedRectTransform);

            // Calculate step length
            float curveLength = CalculateCurveLength(m_curveStepsCount);
            Transform[] cellsTransforms = m_cachedRectTransform.GetActiveChildren();
            int childCount = cellsTransforms.Length;
            float stepLength = (childCount == 1)
                ? 0.5f
                : curveLength / (childCount - 1);

            // Repositing children
            for (int i = 0, l = childCount; i < l; i++)
            {
                Vector2 childPosition = GetPosition(m_curveStepsCount, stepLength * (childCount == 1 ? 1 : i));

                RectTransform cellRectTransform = (RectTransform)cellsTransforms[i];
                cellRectTransform.anchoredPosition = childPosition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepsCount"></param>
        /// <returns></returns>
        private float CalculateCurveLength(int stepsCount)
        {
            float curveLength = 0;

            if (m_animationCurve != null)
            { 
                float timeStep = 1f / stepsCount;
                float timeStepSqr = Mathf.Pow(timeStep, 2f);
                for (int i = 1, l = stepsCount; i < l; i++)
                {
                    curveLength += Mathf.Sqrt(Mathf.Pow(m_animationCurve.Evaluate(timeStep * i) - m_animationCurve.Evaluate(timeStep * (i - 1)), 2f) + timeStepSqr);
                }
            }

            return curveLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepsCount"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private Vector2 GetPosition(int stepsCount, float length)
        {
            float curveLength = 0;

            if (m_animationCurve != null)
            {
                float timeStep = 1f / stepsCount;
                float timeStepSqr = Mathf.Pow(timeStep, 2f);
                for (int i = 1, l = stepsCount; i < l; i++)
                {
                    float curveValue1 = m_animationCurve.Evaluate(timeStep * (i - 1));
                    float curveValue2 = m_animationCurve.Evaluate(timeStep * i);
                    float deltaCurve = Mathf.Sqrt(Mathf.Pow(curveValue2 - curveValue1, 2f) + timeStepSqr);
                    curveLength += deltaCurve;
                    if (curveLength > length
                        || Mathf.Approximately(curveLength, length))
                    {
                        float targetTime = Mathf.Lerp(timeStep * (i - 1), timeStep * i, (curveLength - length) / deltaCurve);
                        float targetValue = m_animationCurve.Evaluate(targetTime);
                        Vector2 relativePosition = (Vector2)m_cachedRectTransform.rect.min
                            + targetTime * m_cachedRectTransform.rect.width * Vector2.right
                            + targetValue * m_cachedRectTransform.rect.height * Vector2.up;
                        return relativePosition;
                    }
                }
            }

            return Vector2.zero;
        }
    }
}