using System.Linq;
using Common.Creators;
using Common.Saves.Providers;
using Dev.Core.Configs;
using Game.Core.GameStateMachine;
using UnityEngine;
using Zenject;

namespace Dev.Core.Installer
{
    public class GameInstaller : Installer<GameInstaller>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ProjectContextRegistration()
        {
            ProjectContext.PreInstall += OnPreInstallProjectContext;
        }

        private static void OnPreInstallProjectContext()
        {
            var normInstallers = ProjectContext.Instance.NormalInstallers.ToList();
            normInstallers.Insert(0, new GameInstaller());
            ProjectContext.Instance.NormalInstallers = normInstallers;
            
            // Add SO installers
            var scInstallers = ProjectContext.Instance.ScriptableObjectInstallers.ToList();
            var coreLoaderConfig = CoreLoaderConfigs.Get();
            scInstallers.Add(coreLoaderConfig);
            ProjectContext.Instance.ScriptableObjectInstallers = scInstallers;

            // Add prefabs installers
            var prefabInstallers = ProjectContext.Instance.InstallerPrefabs.ToList();
            prefabInstallers.AddRange(coreLoaderConfig.PrefabInstallers);
            ProjectContext.Instance.InstallerPrefabs = prefabInstallers;
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<SavesProvider>().AsSingle();
            Container.BindInterfacesTo<ManagerCreator>().AsSingle();
            Container.BindInterfacesTo<GameStateSwitcher>().AsSingle();
        }
    }
}