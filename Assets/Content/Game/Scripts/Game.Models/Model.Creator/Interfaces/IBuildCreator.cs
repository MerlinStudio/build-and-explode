using System;

namespace Model.Creator.Interfaces
{
    public interface IBuildCreator
    {
        bool IsAllAnimationFinished { get; }
        event Action EventEndConstruction;
        void Init(int index);
        void DeInit();
        void SetAmountBlocks(int amountBlocks);
        void CreateBlocks();
    }
}