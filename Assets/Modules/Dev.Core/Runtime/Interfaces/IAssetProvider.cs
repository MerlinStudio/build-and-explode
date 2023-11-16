using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Dev.Core.Interfaces
{
    public interface IAssetProvider
    {
        UniTask<TAsset> TryGetAsset<TAsset>(AssetReference assetReference);
    }
}