using System;
using Dev.Core.Ui.UI.Manager;
using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace Dev.Core.Ui.UI.Panels
{
    // For set prefab mode to UI
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIPanel : MonoBehaviour
    {
        public enum PanelState
        {
            Hide = 0,
            Show = 1
        }

        [SerializeField] protected int m_layerOrder = 1;
        [SerializeField] private UIPanelAnimator m_animator = null;

        [InjectOptional] protected UiManager UIManager;

        public string PanelId { get; set; }
        protected UIPanelData BaseData { get; private set; }
        public PanelState State { get; private set; } = PanelState.Hide;
        public Canvas Canvas { get; set; }
        public CanvasScaler CanvasScaler { get; set; }
        public Camera UiCamera { get; set; }

        public UIPanelAnimator Animator => m_animator;
        public bool IsInstantActive { get; set; }

        public virtual void Initialize(UIPanelData data = null)
        {
            BaseData = data;
        }
    
        public virtual void UpdatePanel(UIPanelData data = null)
        {
            BaseData = data;
        }
    
        public virtual void DeInitialize()
        {
            BaseData = null;
        }

        public virtual void Tick()
        {
        }
    
        public virtual void ShowPanel(bool showInstant = false)
        {
            m_animator.PlayShow(showInstant);
            State = PanelState.Show;
            IsInstantActive = true;
        }

        public virtual void HidePanel(bool hideInstant = false)
        {
            m_animator.PlayHide(hideInstant);
            State = PanelState.Hide;
            IsInstantActive = false;
        }

        public virtual int GetPanelOrder()
        {
            return m_layerOrder;
        }

        public void InvokeHidePanel(Action hideAction = null, bool hideInstant = default)
        {
            UIManager.HidePanel(
                panelGuid: PanelId,
                hideAction: hideAction,
                hideInstant: hideInstant
            );
        }
    }

    public abstract class UiPanel<T> : UIPanel where T : UIPanelData
    {
        public T Data { get; private set; }

        public override void Initialize(UIPanelData data = null)
        {
            base.Initialize(data);

            Assert.IsTrue(data is T, $"Can not cast {data} to {typeof(T)}");
            Data = (T)data;
        }

        public override void DeInitialize()
        {
            Data = null;
            base.DeInitialize();
        }

        public override void UpdatePanel(UIPanelData data = null)
        {
            base.UpdatePanel(data);
            Data = (T)data;
        }
    }
}