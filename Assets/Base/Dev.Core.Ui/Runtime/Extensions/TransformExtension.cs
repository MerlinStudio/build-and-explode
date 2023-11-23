using System.Collections.Generic;
using UnityEngine;

namespace Dev.Core.Ui.Extensions
{
    public static class TransformExtension
    {
        public static Transform[] GetAllChildrenList(this Transform rectTransform)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0, c = rectTransform.childCount; i < c; ++i)
            {
                Transform t = rectTransform.GetChild(i).transform;
                list.Add(t);
            }

            return list.ToArray();
        }

        public static Transform[] GetActiveChildren(this Transform rectTransform)
        {
            List<Transform> activeCells = new List<Transform>();
            Transform[] cells = rectTransform.GetAllChildrenList();
            for (int i = 0, l = cells.Length; i < l; i++)
                if (cells[i].gameObject.activeSelf)
                {
                    //cells[i].name = string.Format("cell_{0:000}", activeCells.Count);
                    activeCells.Add(cells[i]);
                }
            return activeCells.ToArray();
        }

        public static void SetChildrenCount(this Transform rectTransform, int cellsCount)
        {
            Transform[] childList = rectTransform.GetAllChildrenList();
            int childCount = childList.Length;
            int availableCellsCount = childCount - 1;

            if (childCount > 0)
            {
                if (cellsCount > availableCellsCount)
                {
                    for (int i = availableCellsCount; i < cellsCount; i++)
                    {
                        rectTransform.AddChild(childList[0].gameObject);
                    }
                }

                childList = rectTransform.GetAllChildrenList();
                for (int i = 1; i <= cellsCount; i++)
                    childList[i].gameObject.SetActive(true);
                for (int i = cellsCount + 1; i <= availableCellsCount; i++)
                    childList[i].gameObject.SetActive(false);

                if (childList[0].gameObject.activeSelf)
                    childList[0].gameObject.SetActive(false);
            }
        }

        public static GameObject AddChild(this Transform rectTransform, GameObject gObj)
        {
            if (gObj)
            {
                GameObject cloneObj = Object.Instantiate(gObj) as GameObject;
                Transform cloneObjTransform = cloneObj.transform;
                Transform gObjTransform = gObj.transform;
                cloneObjTransform.SetParent(rectTransform);
                cloneObjTransform.localPosition = gObjTransform.localPosition;
                cloneObjTransform.localScale = gObjTransform.localScale;
                cloneObjTransform.localEulerAngles = gObjTransform.localEulerAngles;
                cloneObj.SetActive(true);

                return cloneObj;
            }
            return null;
        }



        #region Monobehaviours
        public static T[] GetAllChildrenList<T>(this Transform rectTransform) where T: Component
        {
            List<T> list = new List<T>();
            for (int i = 0, c = rectTransform.childCount; i < c; ++i)
            {
                T t = rectTransform.GetChild(i).GetComponent<T>();
                if (t)
                    list.Add(t);
            }

            return list.ToArray();
        }

        public static T[] GetActiveChildren<T>(this Transform rectTransform) where T : Component
        {
            List<T> activeCells = new List<T>();
            T[] cells = rectTransform.GetAllChildrenList<T>();
            for (int i = 0, l = cells.Length; i < l; i++)
                if (cells[i].gameObject.activeSelf)
                {
                    //cells[i].name = string.Format("cell_{0:000}", activeCells.Count);
                    activeCells.Add(cells[i]);
                }
            return activeCells.ToArray();
        }

        public static T AddChild<T>(this Transform rectTransform, T t) where T : Component
        {
            if (t)
            {
                T cloneT = Object.Instantiate(t);
                Transform cloneObjTransform = cloneT.transform;
                Transform tTransform = t.transform;
                cloneObjTransform.SetParent(rectTransform);
                cloneObjTransform.localPosition = tTransform.localPosition;
                cloneObjTransform.localScale = tTransform.localScale;
                cloneObjTransform.localEulerAngles = tTransform.localEulerAngles;
                cloneT.gameObject.SetActive(true);

                return cloneT;
            }
            return null;
        }
        #endregion
    }
}
