using Base.Dev.Core.Runtime.Level;
using Cysharp.Threading.Tasks;
using Dev.Core.Interfaces;
using Dev.Core.Level;
using Dev.Core.PanelExample;
using Dev.Core.Ui.UI.Manager;
using UniRx;

namespace Dev.Core.Main
{
    public class PlayerLoopFacade : IPlayerLoopFacade, IPlayerLoopProvider
    {
        public PlayerLoopFacade
        (
            ILevelsProvider levelsProvider,
            ILevelLoader levelLoader,
            UiManager uiManager
        )
        {
            this.levelsProvider = levelsProvider;
            this.levelLoader = levelLoader;
            this.uiManager = uiManager;
        }

        private readonly ILevelsProvider levelsProvider;
        private readonly ILevelLoader levelLoader;
        private readonly UiManager uiManager;

        private CompositeDisposable levelStartCompositeDisposables;
        private CompositeDisposable levelFinishedCompositeDisposables;

        public ISubject<Unit> EventGameLoaded { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelStart { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelLose { get; private set; } = new Subject<Unit>();
        public ISubject<Unit> EventLevelCompleted { get; private set; } = new Subject<Unit>();

        public void Initialize()
        {
            levelStartCompositeDisposables = new CompositeDisposable();
            levelFinishedCompositeDisposables = new CompositeDisposable();

            EventGameLoaded.OnNext(Unit.Default);

            EnterMenu();
        }

        private async void EnterMenu()
        {
            var menuPanel = await uiManager.ShowPanelAsync<MenuPanel>();
            menuPanel.EventStartGame.Subscribe(OnStartLevel).AddTo(levelStartCompositeDisposables);
        }

        private void OnStartLevel(LevelData levelData)
        {
            levelStartCompositeDisposables.Clear();
            uiManager.HidePanel<MenuPanel>();
            EventLevelStart.OnNext(Unit.Default);
            StartLevel(levelData);
        }

        private void StartLevel(LevelData levelData)
        {
            levelLoader.LoadLevel(levelData, level =>
            {
                level.EventLevelResult.Subscribe(OnLevelResult).AddTo(levelStartCompositeDisposables);
                levelsProvider.Level = level;
            });
        }

        private async UniTaskVoid CompleteLevel()
        {
            var resultPanel = await uiManager.ShowPanelAsync<ResultPanel>();
            resultPanel.EventExitLevel.Subscribe(OnExitLevel).AddTo(levelFinishedCompositeDisposables);
        }

        private async UniTaskVoid FailLevel()
        {
            var resultPanel = await uiManager.ShowPanelAsync<ResultPanel>();
            resultPanel.EventContinueLevel.Subscribe(OnContinueLevel).AddTo(levelFinishedCompositeDisposables);
        }
        
        private void OnLevelResult(bool isWin)
        {
            if (isWin)
            {
                CompleteLevel().Forget();
                EventLevelCompleted.OnNext(Unit.Default);
            }
            else
            {
                FailLevel().Forget();
                EventLevelLose.OnNext(Unit.Default);
            }
        }

        private async void OnExitLevel(Unit unit)
        {
            levelFinishedCompositeDisposables.Clear();
            var task1 = uiManager.HidePanelAsync<ResultPanel>();
            await UniTask.WhenAll(task1);
            levelLoader.UnloadCurrentLevel();
            EnterMenu();
        }
        
        private void OnContinueLevel(Unit unit)
        {
            levelFinishedCompositeDisposables.Clear();
            uiManager.HidePanel<ResultPanel>();
            StartLevel(new LevelData());
        }
    }
}