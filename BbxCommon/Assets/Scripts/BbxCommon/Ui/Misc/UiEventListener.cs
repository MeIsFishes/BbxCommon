using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BbxCommon.Ui
{
    public enum EUiEvent
    {
        PointerDown,
        PointerUp,
        PointerEnter,
        PointerExit,
        PointerClick,
        PointerMove,
        Drag,
    }

    public class UiEventListener : MonoBehaviour, IBbxUiItem, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        IPointerMoveHandler, IDragHandler
    {
        public UnityAction<PointerEventData> OnPointerDown;
        public UnityAction<PointerEventData> OnPointerUp;
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnPointerClick;
        public UnityAction<PointerEventData> OnPointerMove;
        public UnityAction<PointerEventData> OnDrag;

        public void AddCallback(EUiEvent uiEvent, UnityAction<PointerEventData> callback)
        {
            switch (uiEvent)
            {
                case EUiEvent.PointerDown:
                    OnPointerDown += callback;
                    break;
                case EUiEvent.PointerUp:
                    OnPointerUp += callback;
                    break;
                case EUiEvent.PointerEnter:
                    OnPointerEnter += callback;
                    break;
                case EUiEvent.PointerExit:
                    OnPointerExit += callback;
                    break;
                case EUiEvent.PointerClick:
                    OnPointerClick += callback;
                    break;
                case EUiEvent.PointerMove:
                    OnPointerMove += callback;
                    break;
                case EUiEvent.Drag:
                    OnDrag += callback;
                    break;
            }
        }

        public void RemoveCallback(EUiEvent uiEvent, UnityAction<PointerEventData> callback)
        {
            switch (uiEvent)
            {
                case EUiEvent.PointerDown:
                    OnPointerDown -= callback;
                    break;
                case EUiEvent.PointerUp:
                    OnPointerUp -= callback;
                    break;
                case EUiEvent.PointerEnter:
                    OnPointerEnter -= callback;
                    break;
                case EUiEvent.PointerExit:
                    OnPointerExit -= callback;
                    break;
                case EUiEvent.PointerClick:
                    OnPointerClick -= callback;
                    break;
                case EUiEvent.PointerMove:
                    OnPointerMove -= callback;
                    break;
                case EUiEvent.Drag:
                    OnDrag -= callback;
                    break;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp?.Invoke(eventData);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick?.Invoke(eventData);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            OnPointerMove?.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);
        }

        void IBbxUiItem.Init(UiControllerBase uiController)
        {
            
        }
    }
}
