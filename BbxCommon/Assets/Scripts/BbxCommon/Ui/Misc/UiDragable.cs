using UnityEngine;
using UnityEngine.EventSystems;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A MonoBehaviour which responses pointer events like moving in, dragging, etc.
    /// </summary>
    public class UiDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
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

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (m_PointerIn)
            {
                m_Dragging = true;
                m_PointerDownPos = eventData.position;
                InitDragData(transform.position, BbxRawTimer.GameTime);
                OnDragBegin(m_DragData);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (m_Dragging)
            {
                m_Dragging = false;
                UpdateDragData(transform.position, BbxRawTimer.GameTime);
                m_DragData.EndPos = transform.position;
                m_DragData.EndTime = BbxRawTimer.GameTime;
                OnDragEnd(m_DragData);
            }
        }
        #endregion
    }
}
