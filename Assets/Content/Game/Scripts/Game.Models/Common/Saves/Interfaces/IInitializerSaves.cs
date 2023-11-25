using Cysharp.Threading.Tasks;

namespace Common.Saves.Interfaces
{
    public interface IInitializerSaves
    {
        UniTask WaitInitYandexSDK();
    }
}