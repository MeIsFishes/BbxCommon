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
        [Tooltip("Objects to be activated when mouse stays in but nor drag.")]
        public List<GameObject> StayItems;
        [FoldoutGroup("Objects in different state"), ShowIf("UseDiffWhenActive")]
        [Tooltip("Objects to be activated when dragging.")]
        public List<GameObject> DragItems;

        // callbacks
        public UnityAction<PointerEventData> OnPointerEnter;
        public UnityAction<PointerEventData> OnPointerStay;
        public UnityAction<PointerEventData> OnPointerExit;
        public UnityAction<PointerEventData> OnPointerDown;
        public UnityAction<PointerEventData> OnPointerUp;
        /// <summary>
        /// Tick on every frame when dragging.
        /// </summary>
        public UnityAction<PointerEventData> OnDrag;

        // internal datas
        [NonSerialized]
        public float DraggedTime;
        private bool m_PointerIn;
        private bool m_Dragging;
        private PointerEventData m_CurrentData;
        private Vector3 m_DragOffset;
        private Vector3 m_StartPos;
        #endregion

        #region CallbacksAndTick
        protected void Awake()
        {
            if (UseDiffWhenActive)
                OnStateInactive();

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
            Debug.Log("ÍË³ö");
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

            OnPointerDown?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            m_Dragging = false;

            if (TurnBackWhenUp)
                transform.position = m_StartPos;

            OnPointerUp?.Invoke(eventData);
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
