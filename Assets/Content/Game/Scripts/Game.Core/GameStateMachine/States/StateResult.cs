using Base.Dev.Core.Runtime.Configs;
using Common.Configs;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
using Game.View.Panels;
using State.Result.Controllers;
using UniRx;

namespace Game.Core.GameStateMachine.States
{
    public class StateResult : AbstractStateBase
    {
        public StateResult(
            IGameStateSwitcher gameStateSwitcher,
            UiManager uiManager,
            LevelsConfig levelsConfig,
            LevelMapConfig levelMapConfig,
            ISavesProvider savesProvider)
        {
            m_gameStateSwitcher = gameStateSwitcher;
            m_uiManager = uiManager;
            m_levelsConfig = levelsConfig;
            m_levelMapConfig = levelMapConfig;
            m_savesProvider = savesProvider;
        }
        
        private readonly IGameStateSwitcher m_gameStateSwitcher;
        private readonly UiManager m_uiManager;
        private readonly LevelsConfig m_levelsConfig;
        private readonly LevelMapConfig m_levelMapConfig;
        private readonly ISavesProvider m_savesProvider;
        
        private LevelMapController m_levelMapController;
        private CompositeDisposable m_compositeDisposable;
        private ISubject<int> m_levelSelected;

        public override void InitState()
        {
            SetLevelProgress();
            SetLevelMap();
        }

        public override void DeinitState()
        {
            m_levelMapController.DeInit();
            m_compositeDisposable.Dispose();
        }

        private void SetLevelProgress()
        {
            var levelProgressController = new LevelProgressController(m_savesProvider);
            levelProgressController.UpdateProgressLevel();
        }

        private async void SetLevelMap()
        {
            var levelMapPanel = await m_uiManager.ShowPanelAsync<LevelMapPanel>();
            m_levelSelected = new Subject<int>();
            m_compositeDisposable = new CompositeDisposable();
            m_levelSelected.Subscribe(OnLevelSelected).AddTo(m_compositeDisposable);
            m_levelMapController = new LevelMapController(levelMapPanel, m_levelsConfig, m_levelMapConfig, m_savesProvider, m_levelSelected);
            m_levelMapController.Init();
        }
        
        private void OnLevelSelected(int selectedLevel)
        {
            m_uiManager.HidePanel<LevelMapPanel>();
            m_savesProvider.SetSavesData<SelectedLevelNumberSaves>(selectedLevel);
            m_gameStateSwitcher.SwitchState<StateLevelLoader>();
        }
    }
}