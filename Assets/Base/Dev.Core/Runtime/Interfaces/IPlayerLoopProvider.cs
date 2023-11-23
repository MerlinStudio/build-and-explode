using UniRx;

namespace Dev.Core.Interfaces
{
    public interface IPlayerLoopProvider
    {
        ISubject<Unit> EventGameLoaded { get; }
        ISubject<Unit> EventLevelStart { get; }
        ISubject<Unit> EventLevelLose { get; }
        ISubject<Unit> EventLevelCompleted { get; }
    }
}