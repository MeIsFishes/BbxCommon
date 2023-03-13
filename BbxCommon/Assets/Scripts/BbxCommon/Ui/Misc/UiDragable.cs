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
    public class UiDragable : MonoBehaviour, IExtendUiItem, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Wrapper
        public struct UiDragableWrapper
        {
            private UiDragable m_Ref;

            public UiDragableWrapper(UiDragable obj) { m_Ref = obj; }

            public UnityAction<PointerEventData> OnPointerEnter { get { return m_Ref.OnPointerEnter; } set { m_Ref.OnPointerEnter = value;} }
            public UnityAction<PointerEventData> OnPointerStay { get { return m_Ref.OnPointerStay; } set { m_Ref.OnPointerStay = value; } }
            public UnityAction<PointerEventData> OnPointerExit { get { return m_Ref.OnPointerExit; } set { m_Ref.OnPointerExit = value; } }
            public UnityAction<PointerEventData> OnBeginDrag { get { return m_Ref.OnBeginDrag; } set { m_Ref.OnBeginDrag = value; } }
            public UnityAction<PointerEventData> OnEndDrag { get { return m_Ref.OnEndDrag; } set { m_Ref.OnEndDrag = value; } }
            /// <summary>
            /// Tick on every frame when dragging.
            /// </summary>
            public UnityAction<PointerEventData> OnDrag { get { return m_Ref.OnDrag; } set { m_Ref.OnDrag = value; } }
        }
        #endregion

        #region Variables
        // public settings
        public bool AlwaysCenter = true;
        [ShowIf("AlwaysCenter")]
        [Tooltip("If true, the object move to mouse's position when it is down.")]
        public bool CenterWhenDown = true;
        [Tooltip("If true, the object will turn back to its start position when dragging ends.")]
        public bool TurnBackWhenDragEnd;

        // callbacks
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerStay;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnBeginDrag;
        public UnityAction<PointerEventData> OnEndDrag;
        public UnityAction<PointerEventData> OnDrag;

        // internal datas
        [NonSerialized]
        public UiControllerBase UiController;
        [NonSerialized]
        public float DraggedTime;
        private bool m_PointerIn;
        private bool m_Dragging;
        private PointerEventData m_CurrentData;
        private Vector3 m_DragOffset;
        private Vector3 m_StartPos;
        private int m_OriginalSiblingIndex;

        // wrapper
        public UiDragableWrapper Wrapper;
        #endregion

        #region CallbacksAndTick
        protected void Awake()
        {
            Wrapper = new UiDragableWrapper(this);
        }

        protected void OnEnable()
        {
            m_StartPos = transform.position;
        }

        protected void Update()
        {
            if (m_PointerIn && !m_Dragging)
                OnPointerStay?.Invoke(m_CurrentData);

            if (m_Dragging)
                DraggedTime += BbxRawTimer.DeltaTime;
        }

        void IExtendUiItem.Init(UiControllerBase uiController)
        {
            UiController = uiController;
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
            if (AlwaysCenter)
                transform.position = eventData.position;
            else
                transform.position = eventData.position.AsVector3XY() + m_DragOffset;

            OnDrag?.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            m_Dragging = true;
            m_DragOffset = transform.position - eventData.position.AsVector3XY();

            if (AlwaysCenter && CenterWhenDown)
                transform.position = eventData.position.AsVector3XY();

            m_OriginalSiblingIndex = UiController.transform.GetSiblingIndex();
            UiController.transform.SetAsLastSibling();

            OnBeginDrag?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            m_Dragging = false;

            if (TurnBackWhenDragEnd)
                transform.position = m_StartPos;

            UiController.transform.SetSiblingIndex(m_OriginalSiblingIndex);

            OnEndDrag?.Invoke(eventData);
        }
        #endregion
    }
}
