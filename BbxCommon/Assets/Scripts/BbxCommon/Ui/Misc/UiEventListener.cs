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
    }

    public class UiEventListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityAction<PointerEventData> OnPointerDown;
        public UnityAction<PointerEventData> OnPointerUp;
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnPointerClick;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {

        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {

        }
    }
}
