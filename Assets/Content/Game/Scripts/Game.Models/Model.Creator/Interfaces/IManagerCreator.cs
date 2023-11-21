using Cysharp.Threading.Tasks;
using Model.Creator.Creators;

namespace Model.Creator.Interfaces
{
    public interface IManagerCreator
    {
        void AddCreator(ICreator creator);
        UniTask<TItem> Create<TItem, TCreator>(string id) where TCreator : ICreator where TItem : CreatedItem;
    }
}