using Creators;
using Cysharp.Threading.Tasks;

namespace State.Creator.Interfaces
{
    public interface IManagerCreator
    {
        void AddCreator(ICreator creator);
        UniTask<TItem> Create<TItem, TCreator>(string id) where TCreator : ICreator where TItem : CreatedItem;
    }
}