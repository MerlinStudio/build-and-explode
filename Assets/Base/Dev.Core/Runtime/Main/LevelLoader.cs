using System;
using Base.Dev.Core.Runtime.Level;
using Cysharp.Threading.Tasks;
using Dev.Core.Interfaces;
using Dev.Core.Level;
using UnityEngine;
using Zenject;

namespace Dev.Core.Main
{
    public class LevelLoader : MonoBehaviour, ILevelLoader
    {
        [SerializeField]
        private Transform levelsRoot;

        [SerializeField]
        private Level.Level defaultLevel;
        
        [Inject]
        private readonly DiContainer diContainer;
        
        [Inject]
        private readonly IAssetProvider assetProvider;

        public Level.Level CurrentLevel { get; private set; }
        
        public void LoadLevel(LevelData levelData, Action<Level.Level> onLoaded = null)
        {
            if (CurrentLevel != null)
            {
                UnloadCurrentLevel();
            }

            if (levelData.UseCustomLevelInfo)
            {
                LoadLevelAsync(levelData, level =>
                {
                    CurrentLevel = level;
                    ShowLevel(levelData);
                    onLoaded?.Invoke(CurrentLevel);
                }).Forget();
            }
            else
            {
                CurrentLevel = defaultLevel;
                CurrentLevel.gameObject.SetActive(true);
                ShowLevel(levelData);
                onLoaded?.Invoke(CurrentLevel);
            }
        }

        public void UnloadCurrentLevel()
        {
            CurrentLevel.Hide();
            CurrentLevel.DeInitialize();

            if (CurrentLevel == defaultLevel)
            {
                CurrentLevel.gameObject.SetActive(false);
            }
            else
            {
                Destroy(CurrentLevel.gameObject);
            }

            CurrentLevel = null;
        }

        private void ShowLevel(LevelData levelData)
        {
            CurrentLevel.Initialize(levelData.LevelSettings);
            CurrentLevel.Show();
        }

        private async UniTask LoadLevelAsync(LevelData levelData, Action<Level.Level> onLoaded)
        {
            var levelReference = levelData.LevelReference;
            var levelPrefab = await assetProvider.TryGetAsset<Level.Level>(levelReference);
            var level = diContainer.InstantiatePrefabForComponent<Level.Level>(levelPrefab, levelsRoot);
            onLoaded?.Invoke(level);
        }
    }
}