using UnityEngine;
using UnityEngine.EventSystems;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A MonoBehaviour which responses pointer events like moving in, dragging, etc.
    /// </summary>
    public class UiDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region UnityTick
        private void Update()
        {
            if (m_Dragging)
            {
                
            }
        }
        #endregion

        #region DragData
        /// <summary>
        /// Stores on-drag related data to invoke callbacks.
        /// Positions in data means GameObject position, but not pointer position.
        /// </summary>
        protected class DragData : PooledObject
        {
            public Vector3 StartPos;
            public float StartTime;
            public Vector3 CurrentPos;
            public float CurrentTime;
            public Vector3 EndPos;
            public float EndTime;

            public override void OnAllocate()
            {
                StartPos = new Vector3();
                StartTime = 0;
                CurrentPos = new Vector3();
                CurrentTime = 0;
                EndPos = new Vector3();
                EndTime = 0;
            }
        }

        protected DragData m_DragData;

        protected void InitDragData(Vector3 startPos, float startTime)
        {
            m_DragData = ObjectPool.AllocIfNull(m_DragData);
            m_DragData.StartPos = startPos;
            m_DragData.StartTime = startTime;
            m_DragData.CurrentPos = startPos;
            m_DragData.CurrentTime = startTime;
        }

        protected void UpdateDragData(Vector3 currentPos, float currentTime)
        {
            m_DragData.CurrentPos = currentPos;
            m_DragData.CurrentTime = currentTime;
        }
        #endregion

        #region DragCallback
        protected virtual void OnDragBegin(DragData data)
        {
            
        }

        protected virtual void OnDragEnd(DragData data)
        {

        }
        #endregion

        #region PointerEvent
        protected bool m_PointerIn = false;
        protected bool m_Dragging = false;
        protected Vector3 m_PointerDownPos;

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            m_PointerIn = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            m_PointerIn = false;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }
        #endregion
    }
}
