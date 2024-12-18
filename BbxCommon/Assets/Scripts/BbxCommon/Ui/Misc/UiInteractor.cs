using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    /// <summary>
    /// Hang this item on a <see cref="Transform"/> root, or instead dragging a <see cref="Transform"/>
    /// to <see cref="TransformOverride"/>, then it will auto select all raycast target under the root,
    /// and get ready for interacting with other <see cref="UiInteractor"/>s.
    /// </summary>
    public class UiInteractor : Interactor, IUiPreInit, IUiInit, IUiDestroy
    {
        #region Wrapper
        [Serializable]
        public struct UiInteractorWrapper
        {
            [SerializeField]
            private UiInteractor m_Ref;
            public UiInteractorWrapper(UiInteractor obj) { m_Ref = obj; }
            /// <summary>
            /// Object for storing interacting information.
            /// </summary>
            public object ExtraInfo { get {  return m_Ref.ExtraInfo; } set { m_Ref.ExtraInfo = value; } }
            /// <summary>
            /// Calls when dragging another interactor and touches.
            /// </summary>
            public UnityAction<Interactor> OnInteractorTouch { get { return m_Ref.OnInteractorTouch; } set { m_Ref.OnInteractorTouch = value; } }
            /// <summary>
            /// Calls when dragging another interactor and touch ends.
            /// </summary>
            public UnityAction<Interactor> OnInteractorTouchEnd { get { return m_Ref.OnInteractorTouchEnd; } set { m_Ref.OnInteractorTouchEnd = value; } }
            public UnityAction<Interactor> OnInteractorAwake { get { return m_Ref.OnInteractorAwake; } set { m_Ref.OnInteractorAwake = value; } }
            /// <summary>
            /// Call OnIteract(requester, responder). For <see cref="UiDragable"/>, the one be dragged is requester.
            /// </summary>
            public UnityAction<Interactor, Interactor> OnInteract { get { return m_Ref.OnInteract; } set { m_Ref.OnInteract = value; } }
            /// <summary>
            /// A syntax sugar. If the current <see cref="Interactor"/> is requester, than pass the responder as parameter, and so do
            /// the opposite case, to free users from thinking of who I am.
            /// </summary>
            public UnityAction<Interactor> OnInteractWith { get { return m_Ref.OnInteractWith; } set { m_Ref.OnInteractWith = value; } }
            public UnityAction OnInteractorSleep { get { return m_Ref.OnInteractorSleep; } set { m_Ref.OnInteractorSleep = value; } }
        }
        #endregion

        #region Common
        public Transform TransformOverride;
        public bool AutoInitUiDragable = true;
        [ShowIf("AutoInitUiDragable")]
        public UiDragable UiDragableRef;
        public UnityAction<Interactor> OnInteractorTouch;
        public UnityAction<Interactor> OnInteractorTouchEnd;

        [HideInInspector]
        public UiInteractorWrapper Wrapper;

        /// <summary>
        /// Raycast targets under the interactor's root. When searching interacting target, GameObjects in the set will be ignored.
        /// </summary>
        private HashSet<GameObject> m_RaycastTargets = new HashSet<GameObject>();

        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new UiInteractorWrapper(this);
            return true;
        }

        void IUiInit.OnUiInit(UiControllerBase uiController)
        {
            if (TransformOverride == null)
                TransformOverride = transform;
            SearchAllRaycastTargets();
            InitUiDragable();
        }

        void IUiDestroy.OnUiDestroy(UiControllerBase uiController)
        {
            UninitUiDragable();
            m_RaycastTargets.Clear();
        }

        private void SearchAllRaycastTargets()
        {
            var graphics = SimplePool<List<Graphic>>.Alloc();
            TransformOverride.GetComponentsInChildren(graphics);
            foreach (var graphic in graphics)
            {
                m_RaycastTargets.TryAdd(graphic.gameObject);
            }
            graphics.CollectToPool();
        }
        #endregion

        #region UiDragable
        private UiInteractor m_Touching;

        public void InteractorTouch(Interactor toucher) { OnInteractorTouch?.Invoke(toucher); }

        public void InteractorTouchEnd(Interactor toucher) { OnInteractorTouchEnd?.Invoke(toucher); }

        private void InitUiDragable()
        {
            // check value
            if (AutoInitUiDragable == false)
                return;
            if (UiDragableRef == null)
                UiDragableRef = TransformOverride.GetComponentInChildren<UiDragable>();
            if (UiDragableRef == null)
            {
                DebugApi.LogError("You set AutoInitUiDragable but there is not such a component on the GameObject!");
                return;
            }
            // init
            UiDragableRef.Wrapper.OnDrag += OnDrag;
            UiDragableRef.Wrapper.OnEndDrag += OnDragEnd;
        }

        private void UninitUiDragable()
        {
            if (AutoInitUiDragable == false)
                return;
            if (UiDragableRef == null)
                UiDragableRef = TransformOverride.GetComponentInChildren<UiDragable>();
            if (UiDragableRef == null)
            {
                DebugApi.LogError("You set AutoInitUiDragable but there is not such a component on the GameObject!");
                return;
            }
            // uninit
            UiDragableRef.Wrapper.OnDrag -= OnDrag;
            UiDragableRef.Wrapper.OnEndDrag -= OnDragEnd;
        }

        private void OnDrag(PointerEventData eventData)
        {
            var results = SimplePool<List<RaycastResult>>.Alloc();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                if (result.gameObject == this.gameObject)   // search for the first other interactor
                    continue;
                if (result.gameObject.TryGetComponent<UiInteractor>(out var uiInteractor))
                {
                    if (m_Touching != uiInteractor)
                    {
                        m_Touching?.InteractorTouchEnd(this);
                        m_Touching = uiInteractor;
                        uiInteractor.InteractorTouch(this);
                    }
                    break;
                }
            }
            results.CollectToPool();
        }

        private void OnDragEnd(PointerEventData eventData)
        {
            var results = SimplePool<List<RaycastResult>>.Alloc();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                if (m_RaycastTargets.Contains(result.gameObject))
                    continue;
                else
                {
                    var responder = result.gameObject.GetComponentInParent<UiInteractor>();
                    // invoke both interactors
                    if (responder != null)
                    {
                        this.Interact(this, responder);
                        responder.Interact(this, responder);
                    }
                    break;
                }
            }
            results.CollectToPool();
        }
        #endregion
    }
}
