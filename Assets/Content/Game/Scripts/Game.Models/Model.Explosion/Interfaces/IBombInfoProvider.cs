using Model.Creator.Controllers;
using Model.Creator.Creators;
using UniRx;

namespace Model.Explosion.Interfaces
{
    public interface IBombInfoProvider
    {
        ReactiveProperty<NewBlockInfo> NewBombInfo { get; set; }
        ReactiveProperty<bool> VisibleNewBombInfo { get; set; }
        void Init();
    }
}