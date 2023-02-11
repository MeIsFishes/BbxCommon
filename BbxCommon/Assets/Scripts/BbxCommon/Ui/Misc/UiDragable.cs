using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A MonoBehaviour which responses pointer events like moving in, dragging, etc.
    /// </summary>
    public class UiDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IDragHandler, IEndDragHandler,IPointerDownHandler
    {
        // public settings
        public bool AlwaysCentered = true;
        [ShowIf("AlwaysCentered")]
        public bool CenterWhenDown = true;

        // callbacks
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerStay;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnPointerDown;
        public UnityAction<PointerEventData> OnDrag;
        public UnityAction<PointerEventData> OnEndDrag;

        // internal datas
        [NonSerialized]
        public float DraggedTime;
        private bool m_PointerIn;
        private bool m_Dragging;
        private PointerEventData m_CurrentData;
        private Vector3 m_DragOffset;

        protected void Update()
        {
            if (m_PointerIn)
                OnPointerStay?.Invoke(m_CurrentData);
            if (m_Dragging)
                DraggedTime += BbxRawTimer.DeltaTime;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_PointerIn = true;
            m_CurrentData = eventData;
            OnPointerEnter?.Invoke(eventData);
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            m_CurrentData = eventData;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_PointerIn = false;
            m_CurrentData = null;
            OnPointerExit?.Invoke(eventData);
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (AlwaysCentered)
                transform.position = eventData.position;
            else
                transform.position = eventData.position.AsVector3XY() + m_DragOffset;
            OnDrag?.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            m_Dragging = false;
            OnEndDrag?.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            m_Dragging = true;
            m_DragOffset = transform.position - eventData.position.AsVector3XY();
            OnPointerDown?.Invoke(eventData);
        }
    }
}
