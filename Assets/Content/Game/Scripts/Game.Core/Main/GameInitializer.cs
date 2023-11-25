using Common.Creators;
using Common.Saves.Interfaces;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
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
        
        [Inject] private UiManager UiManager { get; }
        [Inject] private IInitializerCreator InitializerCreator { get; }
        [Inject] private IInitializerGameState InitializerGameState { get; }
        [Inject] private IInitializerSaves InitializerSaves { get; }

        public async void Start()
        {
            UiManager.ShowPanel<FPSPanel>();
            
            await InitializerSaves.WaitInitYandexSDK();
            InitializerCreator.Init(m_blockCreator, m_particleCreator, m_uiEffectCreator);
            InitializerGameState.Init();
        }

        public void OnDestroy()
        {
            InitializerCreator.DeInit();
            InitializerGameState.DeInit();
        }
    }
}