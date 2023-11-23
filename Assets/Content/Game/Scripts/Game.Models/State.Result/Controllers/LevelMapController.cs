using Configs;
using Cysharp.Threading.Tasks;
using Dev.Core.Level;
using Game.Utils.Extantions;
using Game.View.Components;
using Game.View.Panels;
using UnityEngine;

namespace State.Result.Controllers
{
    public class LevelMapController
    {
        public LevelMapController(
            LevelMapPanel levelMapPanel,
            LevelsConfig levelsConfig,
            LevelMapConfig levelMapConfig)
        {
            m_levelMapPanel = levelMapPanel;
            m_levelsConfig = levelsConfig;
            m_levelMapConfig = levelMapConfig;
        }
        
        private readonly LevelMapPanel m_levelMapPanel;
        private readonly LevelsConfig m_levelsConfig;
        private readonly LevelMapConfig m_levelMapConfig;

        private LevelMapComponent m_levelMapComponentReference;

        public async void Init()
        {
            if (m_levelMapComponentReference == null)
            {
                await SetLevelMapComponent();
            }

            SetLevelMap();
        }

        public void DeInit()
        {
            
        }

        private async UniTask SetLevelMapComponent()
        {
            var levelMapComponentReference = m_levelMapConfig.LevelMapComponentReference;
            var levelMapComponentGameObject = await AssetReferenceExtension.LoadAssetReferenceAsync(levelMapComponentReference);
            m_levelMapComponentReference = levelMapComponentGameObject.GetComponent<LevelMapComponent>();
        }

        private async void SetLevelMap()
        {
            var levelDatas = m_levelsConfig.LevelPacks[0].LevelsData;
            for (int i = 0; i < levelDatas.Count; i++)
            {
                var miniLevelData = levelDatas[i].LevelSettings.MiniLevelData;
                var buildDataConfig = levelDatas[i].LevelSettings.BuildDataConfig;
                var levelMapComponent = m_levelMapPanel.SimpleScrollSnap.AddToFront<LevelMapComponent>(m_levelMapComponentReference);
                if (miniLevelData.MiniLevelObject == null)
                {
                    var miniMapObjectRef = await AssetReferenceExtension.LoadAssetReferenceAsync(miniLevelData.AssetReference);
                    var miniMapObject = Object.Instantiate(miniMapObjectRef);
                    miniLevelData.MiniLevelObject = miniMapObject;
                }
                levelMapComponent.SetMiniLevelObject(miniLevelData.MiniLevelObject);
                levelMapComponent.SetNumberBlocks(buildDataConfig.BlockData.Count);
            }
        }
    }
}