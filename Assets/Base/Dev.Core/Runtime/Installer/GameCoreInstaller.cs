using Common.Creators;
using Common.Saves.Providers;
using Dev.Core.Main;
using Game.Core.GameStateMachine;
using Game.Core.Main;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Dev.Core.Installer
{
    public class GameCoreInstaller : MonoInstaller<GameCoreInstaller>
    {
        [FormerlySerializedAs("m_gameLevel")] [SerializeField]
        private GameInitializer m_gameInitializer;
        
        [SerializeField]
        private CameraManager m_cameraManager;
        
        public override void InstallBindings()
        {
            Container.Bind<CameraManager>().FromInstance(m_cameraManager).AsSingle();
            Container.Bind<GameInitializer>().FromInstance(m_gameInitializer).AsSingle();
        }
    }
}