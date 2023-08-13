using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiInteractor : Interactor, IUiPreInit, IUiInit
    {
        #region Wrapper
        [Serializable]
        public struct UiInteractorWrapper
        {
            [SerializeField]
            private UiInteractor m_Ref;

            public UiInteractorWrapper(UiInteractor obj) { m_Ref = obj; }

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
            /// Call OnIteract(requester, responder).
            /// </summary>
            public UnityAction<Interactor, Interactor> OnInteract { get { return m_Ref.OnInteract; } set { m_Ref.OnInteract = value; } }
            public UnityAction OnInteractorSleep { get { return m_Ref.OnInteractorSleep; } set { m_Ref.OnInteractorSleep = value; } }
        }
        #endregion

        #region Common
        public bool AutoInitUiDragable = true;
        [ShowIf("AutoInitUiDragable")]
        public UiDragable UiDragableRef;
        public UnityAction<Interactor> OnInteractorTouch;
        public UnityAction<Interactor> OnInteractorTouchEnd;

        [HideInEditorMode]
        public UiInteractorWrapper Wrapper;

        /// <summary>
        /// Raycast targets under the interactor's root. When searching interacting target, GameObjects in the set will be ignored.
        /// </summary>
        private HashSet<GameObject> m_RaycastTargets = new HashSet<GameObject>();

        void IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new UiInteractorWrapper(this);
        }

        void IUiInit.OnUiInit(UiControllerBase uiController)
        {
            SearchAllRaycastTargets();
            InitUiDragable();
        }

        private void SearchAllRaycastTargets()
        {
            var graphics = SimplePool<List<Graphic>>.Alloc();
            GetComponentsInChildren(graphics);
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
                UiDragableRef = GetComponent<UiDragable>();
            if (UiDragableRef == null)
            {
                Debug.LogError("You set AutoInitUiDragable but there is not such a component on the GameObject!");
                return;
            }
            // init
            UiDragableRef.Wrapper.OnDrag += OnDrag;
            UiDragableRef.Wrapper.OnEndDrag += OnDragEnd;
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
