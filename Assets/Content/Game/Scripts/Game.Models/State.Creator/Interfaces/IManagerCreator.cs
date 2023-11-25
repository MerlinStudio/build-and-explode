using Common.Creators;
using Cysharp.Threading.Tasks;

namespace State.Creator.Interfaces
{
    public interface IManagerCreator
    {
        UniTask<TItem> Create<TItem, TCreator>(string id) where TCreator : ICreator where TItem : CreatedItem;
    }
}