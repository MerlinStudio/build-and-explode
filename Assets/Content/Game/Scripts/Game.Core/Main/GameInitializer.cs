using Common.Creators;
using Common.Saves.Interfaces;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine;
using Game.Core.GameStateMachine.Interfaces;
using Game.Core.GameStateMachine.States;
using Game.Models.Camera.Interfaces;
using Game.View.Panels;
using State.Creator.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Core.Main
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private BlockCreator m_blockCreator;
        [SerializeField] private ParticleCreator m_particleCreator;
        [SerializeField] private UiEffectCreator m_uiEffectCreator;

        [Inject] private UiManager m_uiManager;
        [Inject] private StateLevelLoader m_stateLevelLoader;
        [Inject] private StateSaveLoader m_stateSaveLoader;
        [Inject] private StateBuild m_stateBuild;
        [Inject] private StateExplosion m_stateExplosion;
        [Inject] private StateResult m_stateResult;
        [Inject] private IInitializerCreator m_initializerCreator;
        [Inject] private IInitializerGameState m_initializerGameState;
        [Inject] private IGameStateSwitcher m_gameStateSwitcher;
        [Inject] private IInitializerSaves m_initializerSaves;
        [Inject] private ICameraController m_cameraController;

        public async void Start()
        {
            m_uiManager.ShowPanel<FPSPanel>();
            
            await m_initializerSaves.WaitInitYandexSDK();
            m_initializerCreator.Init(m_blockCreator, m_particleCreator, m_uiEffectCreator);
            m_initializerGameState.Init(m_stateLevelLoader, m_stateSaveLoader, m_stateBuild, m_stateExplosion, m_stateResult);
            m_gameStateSwitcher.SwitchState<StateLevelLoader>();
            m_cameraController.Init();
        }

        public void OnDestroy()
        {
            m_initializerCreator.DeInit();
            m_initializerGameState.DeInit();
            m_cameraController.DeInit();
        }
    }
}