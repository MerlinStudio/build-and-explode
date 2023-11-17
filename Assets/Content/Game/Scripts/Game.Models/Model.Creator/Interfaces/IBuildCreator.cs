using System;

namespace Model.Creator.Interfaces
{
    public interface IBuildCreator
    {
        event Action EventBuildFinished;
        void Init(int index);
        void DeInit();
        void CreateBlock();
    }
}