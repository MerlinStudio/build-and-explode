using Data.Explosion.Configs;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Manager;
using Game.Data.Models;
using Model.Explosion.Controllers;
using UnityEngine;
using Zenject;

namespace Game.Core.Main
{
    public class GameLevel : Level
    {
        [SerializeField] private CubCreator m_cubCreator;
        [SerializeField] private bool m_isWin;
        
        [Inject] private GameDataModel GameDataModel { get; }
        
        [Inject] private UiManager UiManager { get; }

        public override async void Initialize(LevelSettings levelSettings)
        {
            base.Initialize(levelSettings);

            var buildUIManager = await UiManager.ShowPanelAsync<BuildUIManager>();
            buildUIManager.EventBuildCreateClicked += OnEventBuildCreateClicked;
            buildUIManager.EventExplosionClicked += OnEventExplosionClicked;
        }
        
        public override void DeInitialize()
        {
            base.DeInitialize();
        }

        public override async void Show()
        {
            base.Show();
            
            GameDataModel.PastLevelNumber++;
            GameDataModel.Save();
            Debug.Log($"Level number {GameDataModel.PastLevelNumber}");
            //
            // await Task.Delay(500);
            //
            // Finish(new LevelResultData{IsWin = m_isWin});
        }

        public override void Hide()
        {
            base.Hide();
        }
        
        private void OnEventBuildCreateClicked(BuildDataConfig buildDataConfig)
        {
            m_cubCreator.Build(buildDataConfig);
        }

        private void OnEventExplosionClicked()
        {
            m_cubCreator.Boom();
        }
    }
}