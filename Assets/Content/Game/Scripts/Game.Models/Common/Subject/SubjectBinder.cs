using UniRx;

namespace Game.Models.Common.Subject
{
    public class SubjectBinder<T> : ISubjectBinder<T>
    {
        public ISubject<T> Subject { get; }

        public SubjectBinder()
        {
            Subject = new Subject<T>();
        }
    }
}