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
    public class UiDragable : MonoBehaviour, IBbxUiItem
    {
        #region Wrapper
        [Serializable]
        public struct UiDragableWrapper
        {
            [SerializeField]
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
            public UnityAction<PointerEventData> OnBackFromTop { get { return m_Ref.OnBackFromTop; } set { m_Ref.OnBackFromTop = value; } }
        }
        #endregion

        #region Variables
        // public settings
        [Tooltip("If true, the object will turn back to its start position when dragging ends.")]
        public bool TurnBackWhenDragEnd;

        [FoldoutGroup("RelativeOffset")]
        [Tooltip("If true, item will be moved to a relative position with pointer when dragging.")]
        public bool AlwaysRelativeOffset = true;
        [FoldoutGroup("RelativeOffset"), ShowIf("AlwaysRelativeOffset")]
        public Vector2 RelativeOffset;
        [FoldoutGroup("RelativeOffset"), ShowIf("AlwaysRelativeOffset")]
        [Tooltip("If true, the object move to relative position with pointer when it is pressed down.")]
        public bool SetWhenDown = true;

        [FoldoutGroup("EventListener")]
        public UiEventListener EventListener;
        [FoldoutGroup("EventListener"), Tooltip("The event triggers beginning drag.")]
        public EUiEvent EventBeginDrag = EUiEvent.PointerDown;
        [FoldoutGroup("EventListener"), Tooltip("The event triggers ending drag.")]
        public EUiEvent EventEndDrag = EUiEvent.PointerUp;

        // callbacks
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerStay;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnBeginDrag;
        public UnityAction<PointerEventData> OnEndDrag;
        public UnityAction<PointerEventData> OnDrag;
        public UnityAction<PointerEventData> OnBackFromTop;

        // internal datas
        [NonSerialized]
        public float DraggedTime;
        private bool m_PointerIn;
        private bool m_Dragging;
        private PointerEventData m_CurrentData;
        private Vector3 m_DragOffset;
        private Vector3 m_OriginalPos;

        // wrapper
        [HideInEditorMode]
        public UiDragableWrapper Wrapper;
        #endregion

        #region CallbacksAndTick
        void IBbxUiItem.PreInit(UiViewBase uiView)
        {
            Wrapper = new UiDragableWrapper(this);
        }

        void IBbxUiItem.OnUiInit(UiControllerBase uiController)
        {
            AddCallbacks();
        }

        void IBbxUiItem.OnUiOpen(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiShow(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiHide(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiClose(UiControllerBase uiController) { }

        void IBbxUiItem.OnUiDestroy(UiControllerBase uiController)
        {
            RemoveCallbacks();
        }

        void IBbxUiItem.OnUiUpdate(UiControllerBase uiController)
        {
            if (m_PointerIn && !m_Dragging)
                OnPointerStay?.Invoke(m_CurrentData);

            if (m_Dragging)
                DraggedTime += UiTimer.DeltaTime;
        }

        private void AddCallbacks()
        {
            EventListener.AddCallback(EUiEvent.PointerEnter, OnPointerEnterCallback);
            EventListener.AddCallback(EUiEvent.PointerExit, OnPointerExitCallback);
            EventListener.AddCallback(EUiEvent.PointerMove, OnPointerMoveCallback);
            EventListener.AddCallback(EUiEvent.Drag, OnDragCallback);
            EventListener.AddCallback(EventBeginDrag, OnBeginDragCallback);
            EventListener.AddCallback(EventEndDrag, OnEndDragCallback);
        }

        private void RemoveCallbacks()
        {
            EventListener.RemoveCallback(EUiEvent.PointerEnter, OnPointerEnterCallback);
            EventListener.RemoveCallback(EUiEvent.PointerExit, OnPointerExitCallback);
            EventListener.RemoveCallback(EUiEvent.PointerMove, OnPointerMoveCallback);
            EventListener.RemoveCallback(EUiEvent.Drag, OnDragCallback);
            EventListener.RemoveCallback(EventBeginDrag, OnBeginDragCallback);
            EventListener.RemoveCallback(EventEndDrag, OnEndDragCallback);
        }

        private void OnPointerEnterCallback(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(eventData);

            m_PointerIn = true;
            m_CurrentData = eventData;
        }

        private void OnPointerMoveCallback(PointerEventData eventData)
        {
            m_CurrentData = eventData;
        }

        private void OnPointerExitCallback(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(eventData);

            m_PointerIn = false;
            m_CurrentData = null;
        }


        private void OnDragCallback(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);

            if (AlwaysRelativeOffset)
                transform.position = eventData.position - new Vector2(RelativeOffset.x * transform.localScale.x, RelativeOffset.y * transform.localScale.y);
            else
                transform.position = eventData.position.AsVector3XY() + m_DragOffset;
        }

        private void OnBeginDragCallback(PointerEventData eventData)
        {
            OnBeginDrag?.Invoke(eventData);

            m_OriginalPos = transform.position;

            m_Dragging = true;
            m_DragOffset = transform.position - eventData.position.AsVector3XY();

            if (AlwaysRelativeOffset && SetWhenDown)
                transform.position = (eventData.position - new Vector2(RelativeOffset.x * transform.localScale.x, RelativeOffset.y * transform.localScale.y)).AsVector3XY();

            UiApi.SetUiTop(EventListener.gameObject);
        }

        private void OnEndDragCallback(PointerEventData eventData)
        {
            OnEndDrag?.Invoke(eventData);

            m_Dragging = false;

            if (TurnBackWhenDragEnd)
                transform.position = m_OriginalPos;

            UiApi.SetTopUiBack(EventListener.gameObject);
            OnBackFromTop?.Invoke(eventData);
        }
        #endregion
    }
}
