using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    /// <summary>
    /// A MonoBehaviour which responses pointer events like moving in, dragging, etc.
    /// </summary>
    public class UiDragable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Wrapper
        public struct UiDragableWrapper
        {
            private UiDragable m_Ref;

            public UiDragableWrapper(UiDragable obj) { m_Ref = obj; }

            public UnityAction<PointerEventData> OnPointerEnter { get { return m_Ref.OnPointerEnter; } set { m_Ref.OnPointerEnter = value;} }
            public UnityAction<PointerEventData> OnPointerStay { get { return m_Ref.OnPointerStay; } set { m_Ref.OnPointerStay = value; } }
            public UnityAction<PointerEventData> OnPointerExit { get { return m_Ref.OnPointerExit; } set { m_Ref.OnPointerExit = value; } }
            public UnityAction<PointerEventData> OnDragStart { get { return m_Ref.OnDragStart; } set { m_Ref.OnDragStart = value; } }
            public UnityAction<PointerEventData> OnDragEnd { get { return m_Ref.OnDragEnd; } set { m_Ref.OnDragEnd = value; } }
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
        [Tooltip("If true, the object will turn back to its start position when mouse is up.")]
        public bool TurnBackWhenUp;

        [FoldoutGroup("Objects in different state")]
        [Tooltip("If true, it will activate different objects during different states.")]
        public bool UseDiffWhenActive;
        [FoldoutGroup("Objects in different state"), ShowIf("UseDiffWhenActive")]
        public float ScaleWhenActive;
        [FoldoutGroup("Objects in different state"), ShowIf("UseDiffWhenActive")]
        [Tooltip("Objects to be activated when doesn't interact with mouse.")]
        public List<GameObject> InactiveItems;
        [FoldoutGroup("Objects in different state"), ShowIf("UseDiffWhenActive")]
        [Tooltip("Objects to be activated when mouse stays in but not drag.")]
        public List<GameObject> StayItems;
        [FoldoutGroup("Objects in different state"), ShowIf("UseDiffWhenActive")]
        [Tooltip("Objects to be activated when dragging.")]
        public List<GameObject> DragItems;

        // callbacks
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerStay;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnDragStart;
        public UnityAction<PointerEventData> OnDragEnd;
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
            if (UseDiffWhenActive)
                OnStateInactive();
            Wrapper = new UiDragableWrapper(this);
        }

        protected void OnEnable()
        {
            m_StartPos = transform.position;
        }

        protected void Update()
        {
            if (m_PointerIn && !m_Dragging)
            {
                if (UseDiffWhenActive)
                    OnStateStay();
                OnPointerStay?.Invoke(m_CurrentData);
            }

            if (m_Dragging)
                DraggedTime += BbxRawTimer.DeltaTime;
        }

        public void Init(UiControllerBase uiController)
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
            if (UseDiffWhenActive)
                OnStateInactive();

            m_PointerIn = false;
            m_CurrentData = null;

            OnPointerExit?.Invoke(eventData);
        }


        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (UseDiffWhenActive)
                OnStateDrag();

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

            OnDragStart?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            m_Dragging = false;

            if (TurnBackWhenUp)
                transform.position = m_StartPos;

            UiController.transform.SetSiblingIndex(m_OriginalSiblingIndex);

            OnDragEnd?.Invoke(eventData);
        }
        #endregion

        #region InternalFunctions
        private void OnStateInactive()
        {
            foreach (var obj in StayItems)
                obj.SetActive(false);
            foreach (var obj in DragItems)
                obj.SetActive(false);
            foreach (var obj in InactiveItems)
                obj.SetActive(true);
            transform.localScale = new Vector3(1, 1, 1);
        }

        private void OnStateStay()
        {
            foreach (var obj in InactiveItems)
                obj.SetActive(false);
            foreach (var obj in DragItems)
                obj.SetActive(false);
            foreach (var obj in StayItems)
                obj.SetActive(true);
            transform.localScale = new Vector3(ScaleWhenActive, ScaleWhenActive, ScaleWhenActive);
        }

        private void OnStateDrag()
        {
            foreach (var obj in InactiveItems)
                obj.SetActive(false);
            foreach (var obj in StayItems)
                obj.SetActive(false);
            foreach (var obj in DragItems)
                obj.SetActive(true);
            transform.localScale = new Vector3(ScaleWhenActive, ScaleWhenActive, ScaleWhenActive);
        }
        #endregion
    }
}
