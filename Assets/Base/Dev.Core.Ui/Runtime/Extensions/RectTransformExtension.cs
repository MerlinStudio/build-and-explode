using UnityEngine;

namespace Dev.Core.Ui.Extensions
{
    public static class RectTransformExt
    {
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }

        public static void SetAnchor(this RectTransform rectTransform, Vector3 anchorMin, Vector3 anchorMax)
        {
            Vector3 worldPos = rectTransform.position;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.position = worldPos;
        }
    }
}