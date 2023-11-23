using UnityEngine;
using UnityEngine.UI;

namespace Dev.Core.Ui.Utils
{ 
    public struct SafeAreaSettings
    {
        public string Name;
        public int Height;
        public int Width;
        public bool IsDownButton;
    }

    public enum EScreenDeviceType
    {
        normal = 1,
        monobrow = 2,
        monobrow_and_down = 3
    }

    [ExecuteInEditMode]
    public class UIScreenSafeArea : MonoBehaviour 
    {
        [SerializeField] protected CanvasScaler m_CanvasScaler;
        [SerializeField] protected RectTransform m_TargetRectTransform;
        [SerializeField] protected bool m_LeftSide = true;
        [SerializeField] protected bool m_RightSide = true;
        [SerializeField] protected bool m_UpSide = true;
        [SerializeField] protected bool m_BottomSide = true;

    #if UNITY_EDITOR    
        private static readonly SafeAreaSettings[] SafeAreaSettings = 
        {
            new SafeAreaSettings { Name = "iPhone X Super Retina", Height = 2436, Width = 1125, IsDownButton = true },
            new SafeAreaSettings { Name = "Google Pixel 3 XL",     Height = 2960, Width = 1440, IsDownButton = false }
        };
    
    
        [Header("In Editor")]
        [SerializeField] private bool m_UpdateInEditor = true;


        private void Update()
        {
            if (m_UpdateInEditor)
                ResizeSafeArea();
        }
    #endif

        private void Awake ()
        {
            m_TargetRectTransform = GetComponent<RectTransform>();
            ResizeSafeArea();
	    }

        private void OnEnable()
        {
            // Add listeners
            DeviceChange.OnOrientationChange += OnOrientationChange;
            DeviceChange.OnResolutionChange += OnResolutionChange;
        }

        private void OnDisable()
        {
            // Remove listeners
            DeviceChange.OnOrientationChange -= OnOrientationChange;
            DeviceChange.OnResolutionChange -= OnResolutionChange;
        }

        /// <summary>
        /// Handling changing of orientation
        /// </summary>
        /// <param name="deviceOrientation"></param>
        private void OnOrientationChange(DeviceOrientation deviceOrientation)
        {
            ResizeSafeArea();
        }

        /// <summary>
        /// Handling changing of resolution
        /// </summary>
        /// <param name="resolution"></param>
        private void OnResolutionChange(Vector2 resolution)
        {
            ResizeSafeArea();
        }

        /// <summary>
        /// Resize safe area
        /// </summary>
        private void ResizeSafeArea()
        {
            if (!m_CanvasScaler)
                m_CanvasScaler = transform.GetComponentInParent<CanvasScaler>();
            if (!m_TargetRectTransform)
                m_TargetRectTransform = GetComponent<RectTransform>();

            if (!m_CanvasScaler || !m_TargetRectTransform)
                return;

            Rect safeArea = GetScreenSafeArea();

            float leftOffset = safeArea.x / Screen.width;
            float rightOffset = (Screen.width - safeArea.width - safeArea.x) / Screen.width;
            float upOffset = (Screen.height - safeArea.height - safeArea.y) / Screen.height;
            float downOffset = safeArea.y / Screen.height;

            leftOffset *= m_CanvasScaler.referenceResolution.x;
            rightOffset *= m_CanvasScaler.referenceResolution.x;
            upOffset *= m_CanvasScaler.referenceResolution.y;
            downOffset *= m_CanvasScaler.referenceResolution.y;

            Vector2 newOffsetMin = new Vector2(m_LeftSide ? leftOffset : 0f, m_BottomSide ? downOffset : 0f);
            Vector2 newOffsetMax = new Vector2(m_RightSide ? -rightOffset : 0f, m_UpSide ? -upOffset : 0f);

            m_TargetRectTransform.offsetMin = newOffsetMin;
            m_TargetRectTransform.offsetMax = newOffsetMax;
        }


        /// <summary>
        /// Get safe area of screen
        /// </summary>
        /// <returns></returns>
        private Rect GetScreenSafeArea()
        {
    #if UNITY_EDITOR
            EScreenDeviceType deviceTypeInEditor = EScreenDeviceType.normal;

            for(int i = 0, l = SafeAreaSettings.Length; i < l; i++)
            {
                if (Screen.height != SafeAreaSettings[i].Height || Screen.width != SafeAreaSettings[i].Width)
                    continue;
           
                deviceTypeInEditor = SafeAreaSettings[i].IsDownButton ? EScreenDeviceType.monobrow_and_down : EScreenDeviceType.monobrow;
                break;
            }

            switch (deviceTypeInEditor)
            {
                case EScreenDeviceType.monobrow:
			    case EScreenDeviceType.monobrow_and_down:
				    return new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width - Screen.width * 0.1f, Screen.height - Screen.height * 0.1f);
                case EScreenDeviceType.normal:
                default:
                    return Screen.safeArea;
            }
    #else
            return Screen.safeArea;
    #endif
        }
    }
}