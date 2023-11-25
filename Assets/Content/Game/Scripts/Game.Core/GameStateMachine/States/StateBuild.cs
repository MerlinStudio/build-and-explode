using Common.Configs;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using Dev.Core.Ui.UI.Manager;
using Game.View.Panels;
using State.Creator.Controllers;
using State.Creator.Interfaces;
using State.LevelLoader.Interfaces;

namespace Game.Core.GameStateMachine
{
    public class StateBuild : AbstractStateBase
    {
        public StateBuild(StateBuildDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }

        private readonly StateBuildDependencies m_dependencies;
        
        private BuildClickerPanel m_buildClickerPanel;
        private ClickEffectController m_clickEffectController;
        private int m_savesBlockMultiplier;

        public override async void InitState()
        {
            m_savesBlockMultiplier = m_dependencies.SavesProvider.GetSavesData<BlockMultiplierSaves>();
            await SetBuildClickerPanel();
            SetBuildCreator();
            SetClickEffectController();
        }

        public override void DeinitState()
        {
            m_dependencies.UiManager.HidePanel<BuildClickerPanel>();
            m_buildClickerPanel.EventButtonClicked -= OnCreateBlock;
            m_dependencies.BuildCreator.DeInit();
            m_dependencies.BuildCreator.EventEndConstruction -= OnEndConstruction;
            m_clickEffectController.DeInit();
        }

        private async UniTask SetBuildClickerPanel()
        {
            m_buildClickerPanel = await m_dependencies.UiManager.ShowPanelAsync<BuildClickerPanel>();
            m_buildClickerPanel.EventButtonClicked += OnCreateBlock;
        }
        
        private void SetBuildCreator()
        {
            m_dependencies.BuildCreator.Init();
            m_dependencies.BuildCreator.SetAmountBlocks(m_savesBlockMultiplier);
            m_dependencies.BuildCreator.EventEndConstruction += OnEndConstruction;
        }

        private void SetClickEffectController()
        {
            m_clickEffectController = new ClickEffectController(
                m_dependencies.ManagerCreator,
                m_dependencies.EnvironmentInfoConfig.ClickEffectAnimationInfo);
            m_clickEffectController.Init();
            m_clickEffectController.UpdateAmountBlock(m_savesBlockMultiplier);
        }

        private void OnCreateBlock()
        {
            m_dependencies.BuildCreator.CreateBlocks();
            m_clickEffectController.PlayAddBlockEffect(m_buildClickerPanel.GetAddBlockEffectPosition());
        }

        private async void OnEndConstruction()
        {
            m_dependencies.BuildCreator.EventEndConstruction -= OnEndConstruction;
            m_buildClickerPanel.EventButtonClicked -= OnCreateBlock;

            var fireworkController = new FireworkController(
                m_dependencies.ManagerCreator,
                m_dependencies.LevelProvider);
            var task1 = fireworkController.PlayFirework();
            var task2 = UniTask.WaitWhile(() => m_clickEffectController.IsAllAnimationFinished == false);
            var task3 = UniTask.WaitWhile(() => m_dependencies.BuildCreator.IsAllAnimationFinished == false);
            await UniTask.WhenAll(task1, task2, task3);
            
            m_dependencies.UiManager.HidePanel<BuildClickerPanel>();
            m_dependencies.GameStateSwitcher.SwitchState<StateExplosion>();
        }
        
        public class StateBuildDependencies : StateDependencies
        {
            public UiManager UiManager;
            public IGameStateSwitcher GameStateSwitcher;
            public IBuildCreator BuildCreator;
            public IManagerCreator ManagerCreator;
            public ISavesProvider SavesProvider;
            public ILevelProvider LevelProvider;
            public EnvironmentInfoConfig EnvironmentInfoConfig;
        }
    }
}