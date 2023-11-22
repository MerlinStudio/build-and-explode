using Creators;
using State.Explosion.Interfaces;
using UniRx;

namespace State.Explosion.Controllers
{
    public class BombInfoProvider : IBombInfoProvider
    {
        public ReactiveProperty<NewBlockInfo> NewBombInfo { get; set; }
        public ReactiveProperty<bool> VisibleNewBombInfo { get; set; }

        public void Init()
        {
            NewBombInfo = new ReactiveProperty<NewBlockInfo>();
            VisibleNewBombInfo = new ReactiveProperty<bool>();
        }
    }
}