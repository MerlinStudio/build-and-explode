using Base.Dev.Core.Runtime.Configs;
using Common.Configs;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Dev.Core.Ui.UI.Manager;
using Game.View.Panels;
using State.Result.Controllers;
using UniRx;

namespace Game.Core.GameStateMachine
{
    public class StateResult : AbstractStateBase
    {
        public StateResult(StateResultDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }
        
        private readonly StateResultDependencies m_dependencies;

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
            var levelProgressController = new LevelProgressController(m_dependencies.SavesProvider);
            levelProgressController.UpdateProgressLevel();
        }

        private async void SetLevelMap()
        {
            var levelMapPanel = await m_dependencies.UiManager.ShowPanelAsync<LevelMapPanel>();
            m_levelSelected = new Subject<int>();
            m_compositeDisposable = new CompositeDisposable();
            m_levelSelected.Subscribe(OnLevelSelected).AddTo(m_compositeDisposable);
            m_levelMapController = new LevelMapController(
                levelMapPanel,
                m_dependencies.LevelsConfig,
                m_dependencies.LevelMapConfig,
                m_dependencies.SavesProvider,
                m_levelSelected);
            m_levelMapController.Init();
        }
        
        private void OnLevelSelected(int selectedLevel)
        {
            m_dependencies.UiManager.HidePanel<LevelMapPanel>();
            m_dependencies.SavesProvider.SetSavesData<SelectedLevelNumberSaves>(selectedLevel);
            m_dependencies.GameStateSwitcher.SwitchState<StateLevelLoader>();
        }
        
        public class StateResultDependencies : StateDependencies
        {
            public IGameStateSwitcher GameStateSwitcher;
            public UiManager UiManager;
            public LevelsConfig LevelsConfig;
            public LevelMapConfig LevelMapConfig;
            public ISavesProvider SavesProvider;
        }
    }
}