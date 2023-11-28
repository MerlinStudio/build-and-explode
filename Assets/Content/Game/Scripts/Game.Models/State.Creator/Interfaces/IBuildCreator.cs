using Cysharp.Threading.Tasks;
using UnityEngine;

namespace State.Creator.Interfaces
{
    public interface IBuildCreator
    {
        bool IsAllAnimationFinished { get; }
        void Init();
        void DeInit();
        void SetAmountBlocks(int amountBlocks);
        void CreateBlocks();
        UniTask<Transform> CreateBlock();
    }
}