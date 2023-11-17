using System;
using Data.Builds.Configs;
using Data.Explosion.Configs;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine;
using Game.Data.Models;
using Model.Creator.Controllers;
using Model.Explosion.Controllers;
using Model.Explosion.Interfaces;
using UnityEngine;
using Zenject;

namespace Game.Core.Main
{
    public class GameLevel : Level
    {
        [SerializeField] private CubCreator m_cubCreator;
        [SerializeField] private BlockCreator m_blockCreator;
        [SerializeField] private bool m_isWin;
        
        [Inject] private GameDataModel GameDataModel { get; }
        [Inject] private UiManager UiManager { get; }
        [Inject] private EnvironmentInfoConfig EnvironmentInfoConfig { get; }

        private IGameStateSwitcher m_gameStateSwitcher;
        private ExplosionManager m_explosionManager;
        
        public override void Initialize(LevelSettings levelSettings)
        {
            base.Initialize(levelSettings);

            UiManager.ShowPanel<BuildUIManager>();

            m_gameStateSwitcher = new GameStateSwitcher();
            m_explosionManager = new ExplosionManager();
            var switcherDependencies = new SwitcherDependencies
            {
                UiManager = UiManager,
                GameDataModel = GameDataModel,
                LevelSettings = levelSettings,
                BlockCreator = m_blockCreator,
                ExplosionManager = m_explosionManager,
                EnvironmentInfoConfig = EnvironmentInfoConfig
            };
            m_gameStateSwitcher.Init(switcherDependencies);
            m_gameStateSwitcher.SwitchState<StateBuild>();
        }
        
        public override void DeInitialize()
        {
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
            m_explosionManager.Tick();
        }
    }
}