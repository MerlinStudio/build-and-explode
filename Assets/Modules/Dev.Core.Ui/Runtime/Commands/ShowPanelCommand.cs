using System;
using Dev.Core.Ui.Assets;
using Dev.Core.Ui.UI.Manager;
using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Dev.Core.Ui.Commands
{
    public class ShowPanelCommand : PanelCommand
    {
        public event Action PanelLoadedEvent;
        public bool PanelLoaded { get; private set; }

        private readonly Camera m_uiCamera;
        private readonly UIPanelData m_data;
        private readonly DiContainer m_container;
        private readonly bool m_isInstanceAnim;
        private IDisposable m_disposable;
        private PanelLayer m_panelLayer;

        [Inject] private AssetsProvider m_assetsProvider;
        [Inject] private UiManager m_uiManager;

        public ShowPanelCommand(string panelId, Camera uiCamera, UIPanelData data, DiContainer injectContainer,
            bool isInstanceAnim)
        {
            PanelId = panelId;
            m_uiCamera = uiCamera;
            m_data = data;
            m_container = injectContainer;
            m_isInstanceAnim = isInstanceAnim;
        }

        protected override async void Start()
        {
            var handle = m_assetsProvider.LoadByGuid<GameObject>(PanelId);
            await handle.Task;
            
            if (handle.Status != AsyncOperationStatus.Succeeded || Canceled) return;
            PanelLoadedEvent?.Invoke();
            PanelLoaded = true;
            
            var prefab = handle.Result;
            var goPanel = prefab.GetComponent<UIPanel>();
            var panelOrder = goPanel.GetPanelOrder();
            m_panelLayer = m_uiManager.GetOrCreateLayer(panelOrder);
            var go = m_container.InstantiatePrefab(prefab, m_panelLayer.transform);
            Panel = go.GetComponent<UIPanel>();
            
            var animator = go.GetComponentInChildren<Animator>();
            if (animator != null) animator.Update(0f);
            
            m_panelLayer.AddPanel(Panel);

            Panel.PanelId = PanelId;
            Panel.Canvas = m_panelLayer.Canvas;
            Panel.CanvasScaler = m_panelLayer.CanvasScaler;
            Panel.UiCamera = m_uiCamera;
            Panel.Initialize(m_data);
            
            m_disposable = ObservableExtensions.Subscribe<long>(Observable.EveryUpdate(), CheckPanelState);
            Panel.ShowPanel(m_isInstanceAnim);
        }

        private void CheckPanelState(long l)
        {
            if (Canceled)
            {
                m_panelLayer.RemovePanel(Panel);
                DOTween.Kill(Panel.gameObject);
                m_disposable.Dispose();
                OnCompleted(false);
                return;
            }

            if (Panel.State != UIPanel.PanelState.Show) return;
            
            m_disposable.Dispose();
            OnCompleted(true);
        }

        public class Factory : PlaceholderFactory<string, Camera, UIPanelData, DiContainer, bool, ShowPanelCommand>
        {
        }
    }
}