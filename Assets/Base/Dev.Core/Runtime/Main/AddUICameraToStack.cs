using Dev.Core.Ui.UI.Manager;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Dev.Core.Main
{
    public class AddUICameraToStack : MonoBehaviour
    {
        [SerializeField] private Camera m_cameraWithStack;
        
        [Inject] private UiManager UiManager { get; }

        private Camera m_uiCamera;

        private Camera UiCamera
        {
            get
            {
                if (!m_uiCamera)
                { 
                    m_uiCamera = UiManager.UiCamera;
                }
                if (!m_uiCamera)
                { 
                    GameObject uiCameraGameObject = GameObject.FindGameObjectWithTag("UiCamera");
                    if (uiCameraGameObject)
                        m_uiCamera = uiCameraGameObject.GetComponent<Camera>();
                }

                return m_uiCamera;
            }
        }


        private void Awake()
        {
            UniversalAdditionalCameraData uiCameraUniversalAdditionalCameraData = UiCamera.GetUniversalAdditionalCameraData();
            uiCameraUniversalAdditionalCameraData.renderType = CameraRenderType.Overlay;

            UniversalAdditionalCameraData cameraWithStackUniversalAdditionalCameraData = m_cameraWithStack.GetUniversalAdditionalCameraData();
            cameraWithStackUniversalAdditionalCameraData.cameraStack.Add(UiCamera);
        }

        private void Destroy()
        {
            UniversalAdditionalCameraData cameraWithStackUniversalAdditionalCameraData = m_cameraWithStack.GetUniversalAdditionalCameraData();
            cameraWithStackUniversalAdditionalCameraData.cameraStack.Remove(UiCamera);

            UniversalAdditionalCameraData uiCameraUniversalAdditionalCameraData = UiCamera.GetUniversalAdditionalCameraData();
            uiCameraUniversalAdditionalCameraData.renderType = CameraRenderType.Base;
        }
    }
}