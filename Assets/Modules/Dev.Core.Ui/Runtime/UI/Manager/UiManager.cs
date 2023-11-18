using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.Commands;
using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Dev.Core.Ui.UI.Manager
{
    public class UiManager : MonoBehaviour
    {
        public event Action<UIPanel> EventPanelShow;
        public event Action<UIPanel> EventPanelChange;
        public event Action<UIPanel> EventPanelHide;

        [SerializeField] private Camera m_uiCamera;

        [Header("Layers")] [SerializeField] private Transform m_layersRoot;
        [SerializeField] private PanelLayer m_panelLayerPrefab;

        private readonly Dictionary<int, PanelLayer> m_layers = new Dictionary<int, PanelLayer>();
        private readonly Dictionary<string, UIPanel> m_panels = new Dictionary<string, UIPanel>();
        private readonly List<PanelCommand> m_commandsInProgress = new List<PanelCommand>();
        private Dictionary<string, string> m_panelGuids = new Dictionary<string, string>();

        [Inject] private DiContainer Сontainer { get; }
        [Inject] private LoadPanelCommand.Factory m_loadPanelCommandFactory { get; }
        [Inject] private ShowPanelCommand.Factory m_showPanelCommandFactory { get; }
        [Inject] private HidePanelCommand.Factory m_hidePanelCommandFactory { get; }
        [Inject] private UnloadPanelCommand.Factory m_unloadPanelCommandFactory { get; }
        [Inject] private ChangePanelCommand.Factory m_changePanelCommandFactory { get; }
        [Inject] private UpdatePanelCommand.Factory m_updatePanelCommandFactory { get; }
   
        public Camera UiCamera => m_uiCamera;


        [Inject]
        private void Initialize()
        {
            var attributeType = typeof(UIPanelsGuidProviderAttribute);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var uiPanelsGuidProvider = new List<Type>();
        
            foreach (var typesWithPathProviderAttribute in assemblies.Select(assembly =>
                         (assembly.IsDynamic
                             ? assembly.GetTypes()
                             : assembly.GetExportedTypes()).Where(type => type.IsAbstract && type.IsSealed && type.IsClass
                                                                          && type.GetCustomAttributes(attributeType, false).Any())))
            {
                uiPanelsGuidProvider.AddRange(typesWithPathProviderAttribute);
            }

            if (uiPanelsGuidProvider.Any())
            {
                foreach (var pathProviderType in uiPanelsGuidProvider)
                {
                    var panels = (Dictionary<string, string>) pathProviderType.GetField("Panels").GetValue(null);
            
                    foreach (var keyValuePair in panels)
                    {
                        if (!m_panelGuids.ContainsKey(keyValuePair.Key)) m_panelGuids.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }
            else
            {
                Debug.LogError($"panel guids is null");
                //m_panelGuids = UIPanels.Panels;
            }

            m_uiCamera.tag = "UiCamera";
        }

        private void OnDestroy()
        {
            var hidePanels = new HashSet<string>();
            foreach (var uiPanel in m_panels)
            {
                hidePanels.Add(uiPanel.Key);
            }

            while (hidePanels.Count > 0)
            {
                var panelId = hidePanels.Last();
                hidePanels.Remove(panelId);
                if (!m_panels.ContainsKey(panelId))
                {
                    continue;
                }

                UnloadPanel(panelId);
            }

            hidePanels.Clear();
        }

        private void Update()
        {
            m_panels.Values.ToList().ForEach(panel => panel.Tick());
        }
    
        private void RegisterCommand(PanelCommand command)
        {
            m_commandsInProgress.Add(command);
            command.Completed += (cmd, result) => { m_commandsInProgress.Remove((PanelCommand) cmd); };
        }

        private string GetGuidByPanelType<T>() where T : UIPanel
        {
            var typeFullName = typeof(T).FullName;
            if (typeFullName == null) throw new Exception($"Name null for type {typeof(T)}");
            return m_panelGuids[typeFullName];
        }

        public bool IsShow<T>() where T : UIPanel
        {
            return IsShow(GetGuidByPanelType<T>());
        }

        private bool IsShow(string panelGuid)
        {
            if (m_panels.ContainsKey(panelGuid)) return m_panels[panelGuid].State == UIPanel.PanelState.Show;
            return false;
        }
    
        public void LoadPanel(string panelGuid, Action<bool> onLoadAction = null)
        {
            var loadCommand = m_loadPanelCommandFactory.Create(panelGuid);
            loadCommand.Completed += (command, result) =>
            {
                onLoadAction?.Invoke(result);
            };
            loadCommand.Execute();
        }

        public PanelCommand ShowPanel<T>(UIPanelData data = null, DiContainer injectContainer = null,
            Action showAction = null, bool showInstant = false) where T : UIPanel
        {
            return ShowPanel(GetGuidByPanelType<T>(), data, injectContainer, showAction, showInstant);
        }

        public PanelCommand ShowPanel(string panelGuid, UIPanelData data = null, DiContainer injectContainer = null,
            Action showAction = null, bool showInstant = false)
        {
            if (IsShow(panelGuid))
            {
                var updateCommand = m_updatePanelCommandFactory.Create(m_panels[panelGuid], data);
                RegisterCommand(updateCommand);
                updateCommand.Execute();
                return updateCommand;
            }

            var currentShowCommand = m_commandsInProgress.Find(
                c => c.PanelId.Equals(panelGuid) && c is ShowPanelCommand);
            if (currentShowCommand != null) return currentShowCommand;

            var showCommand = m_showPanelCommandFactory.Create(
                panelGuid, m_uiCamera, data, injectContainer ?? Сontainer, showInstant);
            showCommand.Completed += (c, result) =>
            {
                if (!result) return;

                m_panels[panelGuid] = showCommand.Panel;
                showAction?.Invoke();
                EventPanelShow?.Invoke(showCommand.Panel);
            };

            RegisterCommand(showCommand);
            showCommand.Execute();

            return showCommand;
        }

        public async UniTask<T> ShowPanelAsync<T>(UIPanelData data = null, DiContainer injectContainer = null, bool showInstant = false) where T : UIPanel
        {
            ShowPanel<T>(data, injectContainer, showInstant: showInstant);
            await UniTask.WaitWhile(() => IsShow<T>() == false);
            return TryGetPanel<T>();
        }

        public void HidePanel<T>(Action hideAction = null, bool hideInstant = false) where T : UIPanel
        {
            HidePanel(GetGuidByPanelType<T>(), hideAction, hideInstant);
        }
    
        public void HidePanel(string panelGuid, Action hideAction = null, bool hideInstant = false)
        {
            var hideCommandInProgress = m_commandsInProgress.Find(commandInProgress =>
                commandInProgress.PanelId.Equals(panelGuid) && commandInProgress is HidePanelCommand);
            if (hideCommandInProgress != null) return;
        
            var showCommand = m_commandsInProgress.Find(c => c.PanelId.Equals(panelGuid) && c is ShowPanelCommand);

            if (showCommand != null)
            {
                showCommand.Cancel();
                if (showCommand.Panel)
                {
                    var hidePanelCommand = m_hidePanelCommandFactory.Create(showCommand.Panel, false);
                    hidePanelCommand.Execute();
                }
            }
            if (!m_panels.ContainsKey(panelGuid)) return;

            var uiPanel = m_panels[panelGuid];
            var command = m_hidePanelCommandFactory.Create(uiPanel, hideInstant);
            command.Completed += (с, result) =>
            {
                if (result)
                {
                    hideAction?.Invoke();
                    EventPanelHide?.Invoke(uiPanel);
                }
            };
        
            RegisterCommand(command);
            command.Execute();
            m_panels.Remove(panelGuid);
        }
        
        public async UniTask HidePanelAsync<T>(bool hideInstant = false) where T : UIPanel
        {
            HidePanel<T>(hideInstant: hideInstant);
            await UniTask.WaitWhile(() => IsShow<T>() == true);
        }

        public void UpdatePanel<T>(UIPanelData data = null) where T : UIPanel
        {
            var panelGuid = GetGuidByPanelType<T>();
            if (!m_panels.ContainsKey(panelGuid)) return;
            var updatePanelCommand = m_updatePanelCommandFactory.Create(m_panels[panelGuid], data);
            RegisterCommand(updatePanelCommand);
            updatePanelCommand.Execute();
        }

        public void ChangePanel<T1, T2>(UIPanelData data, DiContainer injectContainer = null, Action changeAction = null,
            bool changeInstance = false) where T1: UIPanel where T2 : UIPanel
        {
            var hidingPanelGuid = GetGuidByPanelType<T1>();
            if (!m_panels.ContainsKey(hidingPanelGuid)) return;

            var hidingPanel = m_panels[hidingPanelGuid];
            var showingPanelGuid = GetGuidByPanelType<T2>();
            var changePanelCommand = m_changePanelCommandFactory.Create(hidingPanel, showingPanelGuid, m_uiCamera, data, 
                injectContainer ?? Сontainer, changeInstance);
        
            changePanelCommand.Completed += (command, result) =>
            {
                changeAction?.Invoke();
                m_panels.Remove(hidingPanelGuid);
                m_panels.Add(showingPanelGuid, changePanelCommand.Panel);
                EventPanelChange?.Invoke(hidingPanel);
            };
        
            RegisterCommand(changePanelCommand);
            changePanelCommand.Execute();
        }
    
        private void UnloadPanel(string panelGuid)
        {
            Assert.IsTrue(m_panels.ContainsKey(panelGuid), $"Panel {panelGuid} is not showing!");

            var panel = m_panels[panelGuid];
            var unloadPanelCommand = m_unloadPanelCommandFactory.Create(panel);
            unloadPanelCommand.Completed += (command, result) =>
            {
                if (m_panels.ContainsKey(panelGuid)) m_panels.Remove(panelGuid);
            };
        
            RegisterCommand(unloadPanelCommand);
            unloadPanelCommand.Execute();
        }

        // TODO: Bad public code
        public PanelLayer GetOrCreateLayer(int order)
        {
            if (m_layers.ContainsKey(order))
            {
                return m_layers[order];
            }
            return CreateLayer(order);
        }

        private PanelLayer CreateLayer(int order)
        {
            var go = Сontainer.InstantiatePrefab(m_panelLayerPrefab, m_layersRoot);
            var layerObject = go.GetComponent<PanelLayer>();
            layerObject.SetLayer(order);
            m_layers[order] = layerObject;
            return layerObject;
        }

        // TODO: Bad public code
        public void TryDestroyLayer(PanelLayer layer)
        {
            if (layer.HasPanel) return;
        
            m_layers.Remove(layer.LayerOrder);
            Destroy(layer.gameObject);
        }

        public T TryGetPanel<T>() where T : UIPanel
        {
            foreach (var kvp in m_panels)
            {
                if (kvp.Value is T panel)
                {
                    return panel;
                }
            }

            return null;
        }

        public T TryGetPanel<T>(string panelGuid) where T : UIPanel
        {
            return TryGetPanel(panelGuid) as T;
        }

        public UIPanel TryGetPanel(string panelGuid)
        {
            m_panels.TryGetValue(panelGuid, out var panel);
            return panel;
        }

        internal void AddPanel(UIPanel panel)
        {
            m_panels.Add(panel.PanelId, panel);
        }
    }
}