using Configs;
using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using Dev.Core.Ui.UI.Manager;
using Model.Creator.Controllers;
using Model.Creator.Interfaces;
using Model.Explosion.Controllers;
using Model.Explosion.Interfaces;
using Panels;
using UniRx;

namespace Game.Core.GameStateMachine
{
    public class StateExplosion : AbstractStateBase
    {
        public StateExplosion(StateExplosionDependencies dependencies) : base(dependencies)
        {
            m_dependencies = dependencies;
        }

        private readonly StateExplosionDependencies m_dependencies;
        private PreparationExplosionPanel m_preparationExplosionPanel;
        private BuildLayerController m_buildLayerController;
        private PreparationExplosion m_preparationExplosion;
        private InspectorBomb m_inspectorBomb;
        private CompositeDisposable m_compositeDisposable;
        private IBombInfoProvider m_bombInfoProvider;

        public override async void InitState()
        {
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
            m_dependencies.UiManager.HidePanel<PreparationExplosionPanel>();
            m_buildLayerController.DeInit();
            m_buildLayerController.EventSelectBombPlace -= m_preparationExplosion.OnSelectBombPlace;
            m_compositeDisposable.Dispose();
            m_inspectorBomb.DeInit();
        }

        private async UniTask SetPreparationExplosionPanel()
        {
            m_preparationExplosionPanel = await m_dependencies.UiManager.ShowPanelAsync<PreparationExplosionPanel>();
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
            m_buildLayerController = new BuildLayerController(
                m_dependencies.ManagerCreator, 
                m_dependencies.BlocksInfoProvider, 
                m_bombInfoProvider);
            m_preparationExplosion = new PreparationExplosion(
                m_dependencies.ManagerCreator, 
                m_buildLayerController,
                m_dependencies.EnvironmentInfoConfig, 
                m_bombInfoProvider);
            m_buildLayerController.Init();
            m_buildLayerController.EventSelectBombPlace += m_preparationExplosion.OnSelectBombPlace;
        }
        
        private void SetInspectorBomb()
        {
            m_inspectorBomb = new InspectorBomb(
                m_dependencies.BuildDataConfig, 
                m_dependencies.ShopConfig,
                m_preparationExplosionPanel);
            m_inspectorBomb.Init();
        }
        
        private void OnExplosionButtonClick()
        {
            m_preparationExplosionPanel.EventExplosionButtonClick -= OnExplosionButtonClick;
            m_dependencies.UiManager.HidePanel<PreparationExplosionPanel>();
            m_buildLayerController.Explosion();
            m_preparationExplosion.Explosion();
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
        
        public class StateExplosionDependencies : StateDependencies
        {
            public UiManager UiManager;
            public EnvironmentInfoConfig EnvironmentInfoConfig;
            public BuildDataConfig BuildDataConfig;
            public ShopConfig ShopConfig;
            public IGameStateSwitcher GameStateSwitcher;
            public IManagerCreator ManagerCreator;
            public IBlocksInfoProvider BlocksInfoProvider;
        }
    }
}