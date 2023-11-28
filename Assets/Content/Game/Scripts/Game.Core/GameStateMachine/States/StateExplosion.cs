using Common.Configs;
using Cysharp.Threading.Tasks;
using Dev.Core.Ui.UI.Manager;
using Game.Core.GameStateMachine.Interfaces;
using Game.Models.Camera.Interfaces;
using Game.Models.Common.Subject;
using Game.View.Panels;
using State.Creator.Interfaces;
using State.Explosion.Controllers;
using State.Explosion.Interfaces;
using State.LevelLoader.Interfaces;
using UniRx;

namespace Game.Core.GameStateMachine.States
{
    public class StateExplosion : AbstractStateBase
    {
        public StateExplosion(
            UiManager uiManager,
            EnvironmentInfoConfig environmentInfoConfig,
            ShopConfig shopConfig,
            ILevelProvider levelProvider,
            IGameStateSwitcher gameStateSwitcher,
            IManagerCreator managerCreator,
            IBlocksInfoProvider blocksInfoProvider,
            ICameraProvider cameraProvider,
            ISubjectBinder<int> onBuildLayerHeight)
        {
            m_uiManager = uiManager;
            m_environmentInfoConfig = environmentInfoConfig;
            m_shopConfig = shopConfig;
            m_levelProvider = levelProvider;
            m_gameStateSwitcher = gameStateSwitcher;
            m_managerCreator = managerCreator;
            m_blocksInfoProvider = blocksInfoProvider;
            m_cameraProvider = cameraProvider;
            m_onBuildLayerHeight = onBuildLayerHeight;
        }
        
        private readonly UiManager m_uiManager;
        private readonly EnvironmentInfoConfig m_environmentInfoConfig;
        private readonly ShopConfig m_shopConfig;
        private readonly ILevelProvider m_levelProvider;
        private readonly IGameStateSwitcher m_gameStateSwitcher;
        private readonly IManagerCreator m_managerCreator;
        private readonly IBlocksInfoProvider m_blocksInfoProvider;
        private readonly ICameraProvider m_cameraProvider;
        private readonly ISubjectBinder<int> m_onBuildLayerHeight;

        private PreparationExplosionPanel m_preparationExplosionPanel;
        private BuildLayerController m_buildLayerController;
        private PreparationExplosion m_preparationExplosion;
        private InspectorBomb m_inspectorBomb;
        private CompositeDisposable m_compositeDisposable;
        private IBombInfoProvider m_bombInfoProvider;
        private ISubject<Unit> m_explosionFinished;

        public override async void InitState()
        {
            m_cameraProvider.SetActiveRotateCamera(false);
            await SetPreparationExplosionPanel();
            SetBombInfoProvider();
            SetPreparationExplosion();
            SetInspectorBomb();
        }

        public override void DeinitState()
        {
            m_preparationExplosionPanel.EventExplosionButtonClick -= OnExplosionButtonClick;
            m_preparationExplosionPanel.EventAddBombButtonClick -= OnAddBombButtonClick;
            m_preparationExplosionPanel.EventBuildingLayerChange -= OnChangeBuildingLayer;
            m_uiManager.HidePanel<PreparationExplosionPanel>();
            m_buildLayerController.DeInit();
            m_buildLayerController.EventSelectBombPlace -= m_preparationExplosion.OnSelectBombPlace;
            m_compositeDisposable.Dispose();
            m_inspectorBomb.DeInit();
        }

        private async UniTask SetPreparationExplosionPanel()
        {
            m_preparationExplosionPanel = await m_uiManager.ShowPanelAsync<PreparationExplosionPanel>();
            m_preparationExplosionPanel.EventExplosionButtonClick += OnExplosionButtonClick;
            m_preparationExplosionPanel.EventAddBombButtonClick += OnAddBombButtonClick;
            m_preparationExplosionPanel.EventBuildingLayerChange += OnChangeBuildingLayer;
        }

        private void SetBombInfoProvider()
        {
            m_compositeDisposable = new CompositeDisposable();
            m_bombInfoProvider = new BombInfoProvider();
            m_bombInfoProvider.Init();
            m_bombInfoProvider.VisibleNewBombInfo.Subscribe(OnVisibilityBombSettingsPanel).AddTo(m_compositeDisposable);
        }

        private void SetPreparationExplosion()
        {
            m_explosionFinished = new Subject<Unit>();
            m_explosionFinished.Subscribe(OnExplosionFinished).AddTo(m_compositeDisposable);
            m_buildLayerController = new BuildLayerController(m_managerCreator, m_blocksInfoProvider,
                m_bombInfoProvider, m_onBuildLayerHeight);
            m_preparationExplosion = new PreparationExplosion(m_managerCreator, m_buildLayerController, 
                m_environmentInfoConfig, m_bombInfoProvider, m_explosionFinished);
            m_buildLayerController.Init();
            m_buildLayerController.EventSelectBombPlace += m_preparationExplosion.OnSelectBombPlace;
        }
        
        private void SetInspectorBomb()
        {
            m_inspectorBomb = new InspectorBomb(m_levelProvider, m_shopConfig, m_preparationExplosionPanel);
            m_inspectorBomb.Init();
        }
        
        private async void OnExplosionButtonClick()
        {
            m_preparationExplosionPanel.EventExplosionButtonClick -= OnExplosionButtonClick;
            m_buildLayerController.Explosion();
            await m_preparationExplosionPanel.PlayCountdownExplosion();
            m_uiManager.HidePanel<PreparationExplosionPanel>();
            m_preparationExplosion.Explosion();
        }

        private void OnExplosionFinished(Unit unit)
        {
            m_gameStateSwitcher.SwitchState<StateResult>();
        }
        
        private void OnAddBombButtonClick(int radius, int force, float delay)
        {
            m_buildLayerController.AddBombToLayer();
            m_preparationExplosion.AddBombInfo(radius, force, delay);
        }

        private void OnChangeBuildingLayer(float value)
        {
            m_buildLayerController.ChangeBuildingLayer(value);
        }

        private void OnVisibilityBombSettingsPanel(bool isVisible)
        {
            m_preparationExplosionPanel.VisibilityBombSettings(isVisible);
        }

        public override void Tick()
        {
            m_preparationExplosion?.Tick();
        }
    }
}