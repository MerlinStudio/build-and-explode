using Common.Creators;
using Cysharp.Threading.Tasks;

namespace State.Creator.Interfaces
{
    public interface ICreator
    {
        void Init();
        void DeInit();
        UniTask<T> Create<T>(string id) where T : CreatedItem;
    }
}