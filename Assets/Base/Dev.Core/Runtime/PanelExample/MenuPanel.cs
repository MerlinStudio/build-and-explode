using Base.Dev.Core.Runtime.Level;
using Dev.Core.Interfaces;
using Dev.Core.Level;
using Dev.Core.Ui.UI.Panels;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dev.Core.PanelExample
{
    public class MenuPanel : UIPanel
    {
        [SerializeField]
        private Button startGameButton;
        
        [Inject]
        private ILevelsProvider levelsProvider;

        public ISubject<LevelData> EventStartGame { get; private set; }

        public override void ShowPanel(bool showInstant = false)
        {
            base.ShowPanel(showInstant);
            EventStartGame = new Subject<LevelData>();
            startGameButton.onClick.AddListener(OnStartGameButtonClick);
        }

        public override void HidePanel(bool hideInstant = false)
        {
            startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            base.HidePanel(hideInstant);
        }

        private void OnStartGameButtonClick()
        {
            startGameButton.interactable = false;
            var levelData = levelsProvider.GetLevelData();
            Debug.Log($"Level id {levelData.LevelID}");

            EventStartGame.OnNext(levelData);
        }
    }
}