using Cysharp.Threading.Tasks;
using Model.Creator.Controllers;

namespace Model.Creator.Interfaces
{
    public interface IBlockCreator
    {
        void Init();
        void DeInit();
        UniTask<BlockCreator.NewBlockInfo> Create(string id);
    }
}