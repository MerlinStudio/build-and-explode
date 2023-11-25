using Common.Creators;
using UniRx;

namespace State.Explosion.Interfaces
{
    public interface IBombInfoProvider
    {
        ReactiveProperty<NewBlockInfo> NewBombInfo { get; set; }
        ReactiveProperty<bool> VisibleNewBombInfo { get; set; }
        void Init();
    }
}