using UniRx;

namespace Game.Models.Common.Subject
{
    public interface ISubjectBinder<T>
    {
        ISubject<T> Subject { get; }
    }
}