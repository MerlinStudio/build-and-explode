using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace State.Explosion.Controllers
{
    public class PointerHandler : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        public readonly ISubject<Transform> EventItemClick = new Subject<Transform>();
        public readonly ISubject<Transform> EventItemPointerDown = new Subject<Transform>();
        public readonly ISubject<Transform> EventItemPointerEnter = new Subject<Transform>();
        public readonly ISubject<Transform> EventItemPointerExit = new Subject<Transform>();
        public readonly ISubject<Transform> EventItemPointerUp = new Subject<Transform>();

        public void OnPointerClick(PointerEventData eventData)
        {
            EventItemClick.OnNext(transform);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            EventItemPointerExit.OnNext(transform);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventItemPointerEnter.OnNext(transform);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EventItemPointerDown.OnNext(transform);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EventItemPointerUp.OnNext(transform);
        }
    }
}