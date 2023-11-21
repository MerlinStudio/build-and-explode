using Cysharp.Threading.Tasks;
using Model.Creator.Controllers;
using Model.Creator.Creators;

namespace Model.Creator.Interfaces
{
    public interface ICreator
    {
        void Init();
        void DeInit();
        UniTask<T> Create<T>(string id) where T : CreatedItem;
    }
}