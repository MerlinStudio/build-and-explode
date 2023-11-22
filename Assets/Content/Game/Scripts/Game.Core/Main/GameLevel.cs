using Configs;
using Creators;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine;
using Game.Data.Models;
using UnityEngine;
using Zenject;

namespace Game.Core.Main
{
    public class GameLevel : Level
    {
        [SerializeField] private BlockCreator m_blockCreator;
        [SerializeField] private ParticleCreator m_particleCreator;
        [SerializeField] private UiEffectCreator m_uiEffectCreator;
        [SerializeField] private bool m_isWin;
        
        [Inject] private GameDataModel GameDataModel { get; }
        [Inject] private UiManager UiManager { get; }
        [Inject] private EnvironmentInfoConfig EnvironmentInfoConfig { get; }
        [Inject] private ShopConfig ShopConfig { get; }

        private GameStateSwitcher m_gameStateSwitcher;
        private ManagerCreator m_managerCreator;

        public override void Initialize(LevelSettings levelSettings)
        {
            base.Initialize(levelSettings);

            UiManager.ShowPanel<BuildUIManager>();

            m_managerCreator = new ManagerCreator();
            m_managerCreator.Init(m_blockCreator, m_particleCreator, m_uiEffectCreator);
            
            m_gameStateSwitcher = new GameStateSwitcher();
            var switcherDependencies = new SwitcherDependencies
            {
                UiManager = UiManager,
                GameDataModel = GameDataModel,
                LevelSettings = levelSettings,
                ManagerCreator = m_managerCreator,
                EnvironmentInfoConfig = EnvironmentInfoConfig,
                ShopConfig = ShopConfig
            };
            m_gameStateSwitcher.Init(switcherDependencies);
        }
        
        public override void DeInitialize()
        {
            m_managerCreator.DeInit();
            base.DeInitialize();
        }

        public override void Show()
        {
            base.Show();
            
            // GameDataModel.PastLevelNumber++;
            // GameDataModel.Save();
            // Debug.Log($"Level number {GameDataModel.PastLevelNumber}");
            //
            // await Task.Delay(500);
            //
            // Finish(new LevelResultData{IsWin = m_isWin});
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void Update()
        {
            m_gameStateSwitcher?.Tick();
        }
    }
}