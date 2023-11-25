using Base.Dev.Core.Runtime.Configs;
using Base.Dev.Core.Runtime.Level;
using Common.Configs;
using Common.Saves.Controllers;
using Common.Saves.Interfaces;
using Cysharp.Threading.Tasks;
using Data.Builds.Configs;
using Game.Utils.Extantions;
using Game.View.Components;
using Game.View.Panels;
using UniRx;
using UnityEngine;

namespace State.Result.Controllers
{
    public class LevelMapController
    {
        public LevelMapController(
            LevelMapPanel levelMapPanel,
            LevelsConfig levelsConfig,
            LevelMapConfig levelMapConfig,
            ISavesProvider savesProvider,
            ISubject<int> levelSelected)
        {
            m_levelMapPanel = levelMapPanel;
            m_levelsConfig = levelsConfig;
            m_levelMapConfig = levelMapConfig;
            m_savesProvider = savesProvider;
            m_levelSelected = levelSelected;
        }
        
        private readonly LevelMapPanel m_levelMapPanel;
        private readonly LevelsConfig m_levelsConfig;
        private readonly LevelMapConfig m_levelMapConfig;
        private readonly ISavesProvider m_savesProvider;
        private readonly ISubject<int> m_levelSelected;

        private LevelMapComponent m_levelMapComponentReference;
        private GameObject m_lockedMiniMapReference;
        private int m_currentLevelIndex;
        private int m_pastLevelNumber;

        public async void Init()
        {
            if (m_levelMapComponentReference == null)
            {
                await SetLevelMapComponentReference();
            }
            if (m_lockedMiniMapReference == null)
            {
                await SetLockedMiniMapReference();
            }
            
            SetLevelMap();
            m_levelMapPanel.EventSelectLevelIndex += OnLevelSelected;
            m_levelMapPanel.SimpleScrollSnap.EventBeginScrolling += OnBeginScrolling;
            m_levelMapPanel.SimpleScrollSnap.OnPanelCentered.AddListener(OnCurrentLevelView);
        }

        public void DeInit()
        {
            m_levelMapPanel.EventSelectLevelIndex -= OnLevelSelected;
            m_levelMapPanel.SimpleScrollSnap.EventBeginScrolling -= OnBeginScrolling;
            m_levelMapPanel.SimpleScrollSnap.OnPanelCentered.RemoveListener(OnCurrentLevelView);
        }

        private async UniTask SetLevelMapComponentReference()
        {
            var levelMapComponentReference = m_levelMapConfig.LevelMapComponentReference;
            var levelMapComponentGameObject = await AssetReferenceExtension.LoadAssetReferenceAsync(levelMapComponentReference);
            m_levelMapComponentReference = levelMapComponentGameObject.GetComponent<LevelMapComponent>();
        }
        
        private async UniTask SetLockedMiniMapReference()
        {
            var lockMiniMapReference = m_levelMapConfig.LockedMiniMapReference;
            m_lockedMiniMapReference = await AssetReferenceExtension.LoadAssetReferenceAsync(lockMiniMapReference);
        }

        private async void SetLevelMap()
        {
            m_pastLevelNumber = m_savesProvider.GetSavesData<PastLevelNumberSaves>();
            var levelDatas = m_levelsConfig.LevelPacks[0].LevelsData;
            for (int i = levelDatas.Count - 1; i >= 0; i--)
            {
                var miniLevelData = levelDatas[i].LevelSettings.MiniLevelData;
                var buildDataConfig = levelDatas[i].LevelSettings.BuildDataConfig;
                var levelMapComponent = m_levelMapPanel.SimpleScrollSnap.AddToFront<LevelMapComponent>(m_levelMapComponentReference);
                if (m_pastLevelNumber <= i)
                {
                    SetLockedLevelMap(levelMapComponent);
                    continue;
                }

                await SetOpenedLevelMap(levelMapComponent, miniLevelData, buildDataConfig);
            }

            var numberPanel = m_pastLevelNumber % m_levelsConfig.LevelPacks[0].LevelsData.Count;
            Debug.Log($"Start number panel {numberPanel}");
            m_levelMapPanel.SimpleScrollSnap.GoToPanel(numberPanel);
        }

        private void SetLockedLevelMap(LevelMapComponent levelMapComponent)
        {
            var miniMapObject = Object.Instantiate(m_lockedMiniMapReference);
            levelMapComponent.SetMiniLevelObject(miniMapObject);
            levelMapComponent.SetNumberBlocks(0);
        }

        private async UniTask SetOpenedLevelMap(LevelMapComponent levelMapComponent, MiniLevelData miniLevelData, BuildDataConfig buildDataConfig)
        {
            if (miniLevelData.MiniLevelObject == null)
            {
                var miniMapObjectRef = await AssetReferenceExtension.LoadAssetReferenceAsync(miniLevelData.AssetReference);
                var miniMapObject = Object.Instantiate(miniMapObjectRef);
                miniLevelData.MiniLevelObject = miniMapObject;
            }
            levelMapComponent.SetMiniLevelObject(miniLevelData.MiniLevelObject);
            levelMapComponent.SetNumberBlocks(buildDataConfig.BlockData.Count);
        }

        private void OnLevelSelected()
        {
            m_levelSelected.OnNext(m_currentLevelIndex);
        }
        
        private void OnBeginScrolling(bool isBeginScrolling)
        {
            if (isBeginScrolling)
            {
                m_levelMapPanel.SetInteractableButton(false);
            }
   
            Debug.Log($"OnBeginScrolling {m_currentLevelIndex}");
        }
        
        private void OnCurrentLevelView(int current, int next)
        {
            m_currentLevelIndex = current;
            m_levelMapPanel.SetInteractableButton(m_currentLevelIndex <= m_pastLevelNumber);

            Debug.Log($"OnCurrentLevelView {current}");
        }
    }
}