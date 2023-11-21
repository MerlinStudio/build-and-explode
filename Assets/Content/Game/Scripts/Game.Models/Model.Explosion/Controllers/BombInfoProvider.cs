using Model.Creator.Controllers;
using Model.Creator.Creators;
using Model.Explosion.Interfaces;
using UniRx;

namespace Model.Explosion.Controllers
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