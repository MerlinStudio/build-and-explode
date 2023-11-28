using Common.Configs;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
using Game.Models.Common.Subject;
using Game.View.Panels;
using State.Creator.Controllers;
using State.Creator.Interfaces;
using State.LevelLoader.Interfaces;
using UniRx;

namespace Game.Core.GameStateMachine.States
{
    public class StateBuild : AbstractStateBase
    {
        public StateBuild(
            UiManager uiManager,
            IGameStateSwitcher gameStateSwitcher,
            IBuildCreator buildCreator,
            IManagerCreator managerCreator,
            ISavesProvider savesProvider,
            ILevelProvider levelProvider,
            EnvironmentInfoConfig environmentInfoConfig,
            ISubjectBinder<Unit> onEndConstruction)
        {
            m_uiManager = uiManager;
            m_gameStateSwitcher = gameStateSwitcher;
            m_buildCreator = buildCreator;
            m_managerCreator = managerCreator;
            m_savesProvider = savesProvider;
            m_levelProvider = levelProvider;
            m_environmentInfoConfig = environmentInfoConfig;
            m_onEndConstruction = onEndConstruction;
        }
        
        private readonly UiManager m_uiManager;
        private readonly IGameStateSwitcher m_gameStateSwitcher;
        private readonly IBuildCreator m_buildCreator;
        private readonly IManagerCreator m_managerCreator;
        private readonly ISavesProvider m_savesProvider;
        private readonly ILevelProvider m_levelProvider;
        private readonly EnvironmentInfoConfig m_environmentInfoConfig;
        private readonly ISubjectBinder<Unit> m_onEndConstruction;

        private CompositeDisposable m_compositeDisposable;
        private BuildClickerPanel m_buildClickerPanel;
        private ClickEffectController m_clickEffectController;
        private int m_savesBlockMultiplier;

        public override async void InitState()
        {
            m_savesBlockMultiplier = m_savesProvider.GetSavesData<BlockMultiplierSaves>();
            m_compositeDisposable = new CompositeDisposable();
            await SetBuildClickerPanel();
            SetBuildCreator();
            SetClickEffectController();
        }

        public override void DeinitState()
        {
            m_uiManager.HidePanel<BuildClickerPanel>();
            m_buildCreator.DeInit();
            m_clickEffectController.DeInit();
            m_compositeDisposable.Dispose();
        }

        private async UniTask SetBuildClickerPanel()
        {
            m_buildClickerPanel = await m_uiManager.ShowPanelAsync<BuildClickerPanel>();
            m_buildClickerPanel.OnButtonClicked.Subscribe(OnCreateBlock).AddTo(m_compositeDisposable);
        }
        
        private void SetBuildCreator()
        {
            m_buildCreator.Init();
            m_buildCreator.SetAmountBlocks(m_savesBlockMultiplier);
            m_onEndConstruction.Subject.Subscribe(OnEndConstruction).AddTo(m_compositeDisposable);
        }

        private void SetClickEffectController()
        {
            m_clickEffectController = new ClickEffectController(m_managerCreator, m_environmentInfoConfig.ClickEffectAnimationInfo);
            m_clickEffectController.Init();
            m_clickEffectController.UpdateAmountBlock(m_savesBlockMultiplier);
        }

        private void OnCreateBlock(Unit unit)
        {
            m_buildCreator.CreateBlocks();
            m_clickEffectController.PlayAddBlockEffect(m_buildClickerPanel.GetAddBlockEffectPosition());
        }

        private async void OnEndConstruction(Unit unit)
        {
            m_compositeDisposable.Dispose();

            var fireworkController = new FireworkController(m_managerCreator, m_levelProvider);
            var task1 = fireworkController.PlayFirework();
            var task2 = UniTask.WaitWhile(() => m_clickEffectController.IsAllAnimationFinished == false);
            var task3 = UniTask.WaitWhile(() => m_buildCreator.IsAllAnimationFinished == false);
            await UniTask.WhenAll(task1, task2, task3);
            
            m_uiManager.HidePanel<BuildClickerPanel>();
            m_gameStateSwitcher.SwitchState<StateExplosion>();
        }
    }
}