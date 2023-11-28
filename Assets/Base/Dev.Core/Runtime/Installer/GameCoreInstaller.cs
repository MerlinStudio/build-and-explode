using Common.Creators;
using Common.Saves.Providers;
using Game.Core.GameStateMachine;
using Game.Core.GameStateMachine.States;
using Game.Core.Main;
using Game.Models.Camera.Components;
using Game.Models.Camera.Controllers;
using Game.Models.Common.Subject;
using Game.Models.State.Creator.Controllers;
using State.Creator.Controllers;
using State.LevelLoader.Providers;
using State.SavaLoader.Controllers;
using UnityEngine;
using Zenject;

namespace Dev.Core.Installer
{
    public class GameCoreInstaller : MonoInstaller<GameCoreInstaller>
    {
        [SerializeField]
        private GameInitializer m_gameInitializer;
        
        [SerializeField]
        private TargetGroupCamera targetGroupCamera;
        
        public override void InstallBindings()
        {
            Container.Bind(typeof(ISubjectBinder<>)).To(typeof(SubjectBinder<>)).AsSingle();
            Container.Bind<TargetGroupCamera>().FromInstance(targetGroupCamera).AsSingle();

            Container.BindInterfacesTo<SavesProvider>().AsSingle();
            Container.BindInterfacesTo<LevelProvider>().AsSingle();
            Container.BindInterfacesTo<ManagerCreator>().AsSingle();
            Container.BindInterfacesTo<СonstructionСontroller>().AsSingle();
            Container.BindInterfacesTo<SaveConstructionController>().AsSingle();
            Container.BindInterfacesTo<CameraController>().AsSingle();

            Container.BindInterfacesTo<GameStateSwitcher>().AsSingle();
            Container.Bind<StateLevelLoader>().AsSingle();
            Container.Bind<StateSaveLoader>().AsSingle();
            Container.Bind<StateBuild>().AsSingle();
            Container.Bind<StateExplosion>().AsSingle();
            Container.Bind<StateResult>().AsSingle();

            Container.Bind<GameInitializer>().FromInstance(m_gameInitializer).AsSingle();
        }
    }
}