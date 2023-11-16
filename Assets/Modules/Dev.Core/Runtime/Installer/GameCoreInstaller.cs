using Dev.Core.Main;
using UnityEngine;
using Zenject;

namespace Dev.Core.Installer
{
    public class GameCoreInstaller : MonoInstaller<GameCoreInstaller>
    {
        [SerializeField]
        private GameCoreInitializer gameCoreInitializer;

        [SerializeField]
        private LevelLoader levelLoader;
        
        [SerializeField]
        private CameraManager cameraManager;
        
        public override void InstallBindings()
        {
            Container.Bind<CameraManager>().FromInstance(cameraManager).AsSingle();
            Container.BindInterfacesTo<GameCoreInitializer>().FromInstance(gameCoreInitializer).AsSingle();
            Container.BindInterfacesTo<LevelLoader>().FromInstance(levelLoader).AsSingle();
            Container.BindInterfacesTo<PlayerLoopFacade>().AsSingle();
        }
    }
}