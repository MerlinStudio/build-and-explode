using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;

namespace Dev.Core.Ui.Assets
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AssetsProvider
    {
        public event Action PreloadCompleted;
        private const string PRELOAD_LABEL = "Preload";

        public async void PreloadAssets()
        {
            var loadLocationsTask = Addressables.LoadResourceLocationsAsync(PRELOAD_LABEL).Task;
            await loadLocationsTask;

            var tasks = loadLocationsTask.Result.Select(LoadAsset).Where(task => task != null).ToList();
            await Task.WhenAll(tasks);

            PreloadCompleted?.Invoke();
        }

        private Task<TObject> LoadAsset<TObject>(IResourceLocation resourceLocation)
        {
            return Addressables.LoadAssetAsync<TObject>(resourceLocation).Task;
        }

        private Task LoadAsset(IResourceLocation resourceLocation)
        {
            if (resourceLocation.ResourceType == typeof(GameObject))
            {
                return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(resourceLocation).Task;
            }

            if (resourceLocation.ResourceType == typeof(AudioClip))
            {
                return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<AudioClip>(resourceLocation).Task;
            }

            if (resourceLocation.ResourceType == typeof(SpriteAtlas))
            {
                return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<SpriteAtlas>(resourceLocation).Task;
            }

            if (resourceLocation.ResourceType == typeof(Sprite))
            {
                return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>(resourceLocation).Task;
            }

            if (resourceLocation.ResourceType == typeof(TextAsset))
            {
                return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TextAsset>(resourceLocation).Task;
            }

            return null;
        }

        public AsyncOperationHandle<T> LoadByGuid<T>(string assetGuid)
        {
            return Addressables.LoadAssetAsync<T>(new AssetReference(assetGuid));
        }

        public AsyncOperationHandle<T> LoadByName<T>(string assetName)
        {
            return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(assetName);
        }
    }
}