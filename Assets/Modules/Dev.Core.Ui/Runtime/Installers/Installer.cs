using Dev.Core.Ui.Assets;
using Dev.Core.Ui.Commands;
using Dev.Core.Ui.UI.Manager;
using Dev.Core.Ui.UI.Panels;
using Dev.Core.Ui.UI.Panels.Data;
using UnityEngine;
using Zenject;

namespace Dev.Core.Ui.Installers
{
    public class Installer : MonoInstaller
    {
        [SerializeField] private UiManager uiManagerPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<AssetsProvider>().AsSingle();

            Container.BindInterfacesAndSelfTo<UiManager>().FromComponentInNewPrefab(uiManagerPrefab).AsSingle();

            Container.BindFactory<string, LoadPanelCommand, LoadPanelCommand.Factory>();
            Container.BindFactory<string, Camera, UIPanelData, DiContainer,
                bool, ShowPanelCommand, ShowPanelCommand.Factory>();
            Container.BindFactory<UIPanel, bool, HidePanelCommand, HidePanelCommand.Factory>();
            Container.BindFactory<UIPanel, UnloadPanelCommand, UnloadPanelCommand.Factory>();
            Container.BindFactory<UIPanel, string, Camera, UIPanelData, DiContainer, bool, ChangePanelCommand,
                ChangePanelCommand.Factory>();
            Container.BindFactory<UIPanel, UIPanelData, UpdatePanelCommand, UpdatePanelCommand.Factory>();
        }
    }
}