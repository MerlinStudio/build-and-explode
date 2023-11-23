using Game.Data;
using UniRx;
using UnityEngine;

namespace Dev.Core.Level
{
    public abstract class Level : MonoBehaviour
    {
        public bool IsShow { get; private set; }
        public ISubject<LevelResultData> EventLevelResult { get; private set; }

        public virtual void Initialize(LevelSettings levelSettings)
        {
            EventLevelResult = new Subject<LevelResultData>();
        }

        public virtual void DeInitialize()
        {
            EventLevelResult = null;
        }

        public virtual void Show()
        {
            if(IsShow) return;
            
            IsShow = true;
        }

        public virtual void Hide()
        {
            if(IsShow == false) return;

            IsShow = false;
        }

        public virtual void Finish(LevelResultData levelResultData)
        {
            EventLevelResult?.OnNext(levelResultData);
        }
    }
}