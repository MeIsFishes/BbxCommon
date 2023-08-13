using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public class UiList : MonoBehaviour, IUiPreInit, IUiInit, IUiOpen, IUiShow, IUiHide, IUiClose, IUiDestroy, IUiUpdate
    {
        [Serializable]
        public struct UiListWrapper
        {
            [SerializeField]
            private UiList m_Ref;

            public UiListWrapper(UiList obj) { m_Ref = obj; }

            public T CreateItem<T>() where T : UiControllerBase, IUiListItem => m_Ref.CreateItem<T>();
            public void RemoveItem(int index) => m_Ref.RemoveItem(index);
            public void RemoveItem(IUiListItem item) => m_Ref.RemoveItem(item);
            public void ClearItems() => m_Ref.ClearItems();
        }

        private struct ProtoInfo
        {
            public Vector2 Size;
        }

        public enum EArrangement
        {
            /// <summary>
            /// Giving numbers of a single line and padding space, set each item to the calculated slot.
            /// </summary>
            ConstantSlot,
            /// <summary>
            /// Giving a area space, spread items evenly on it.
            /// </summary>
            AreaFit,
        }

        public enum EDirection
        {
            Horizontal,
            Vertical,
        }

        [HideInInspector]
        public UiListWrapper Wrapper;
        [Tooltip("Proto view to create items of the list.")]
        public UiViewBase ItemProto;
        [InfoBox("ConstantSlot: Giving numbers of a single line and padding space, set each item to the calculated slot." +
            "\nAreaFit: Spread items evenly on the area space, which identical to the RectTransform.size value.")]
        public EArrangement ArragementType;
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public EDirection AreaDirection;
        [ShowIf("@ArragementType == EArrangement.AreaFit"), Tooltip("Padding space between two items.")]
        public float PaddingSpace;

        private List<IUiListItem> m_UiItems = new List<IUiListItem>();
        private bool m_ProtoInfoInited;
        private ProtoInfo m_ProtoInfo;

        void IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new UiListWrapper(this);
        }

        void IUiInit.OnUiInit(UiControllerBase uiController) { }

        void IUiOpen.OnUiOpen(UiControllerBase uiController) { }

        void IUiShow.OnUiShow(UiControllerBase uiController)
        {
            foreach (var item in m_UiItems)
            {
                ((UiControllerBase)item).Show();
            }
        }

        void IUiHide.OnUiHide(UiControllerBase uiController)
        {
            foreach (var item in m_UiItems)
            {
                ((UiControllerBase)item).Hide();
            }
        }

        void IUiClose.OnUiClose(UiControllerBase uiController)
        {
            ClearItems();
        }

        void IUiDestroy.OnUiDestroy(UiControllerBase uiController)
        {
            foreach (var item in m_UiItems)
            {
                ((UiControllerBase)item).Destroy();
            }
        }

        void IUiUpdate.OnUiUpdate(UiControllerBase uiController, float deltaTime) { }

        private void Refresh()
        {
            switch (ArragementType)
            {
                case EArrangement.ConstantSlot:
                    RefreshConstantSlot();
                    break;
                case EArrangement.AreaFit:
                    RefreshAreaFit();
                    break;
            }
        }

        private void RefreshConstantSlot()
        {

        }

        private void RefreshAreaFit()
        {
            if (m_UiItems.Count == 0)
                return;

            var rect = ((RectTransform)transform).rect;
            if (m_UiItems.Count == 1)   // keep it in center
                ((UiControllerBase)m_UiItems[0]).transform.position = rect.position;

            switch (AreaDirection)
            {
                case EDirection.Horizontal:
                    float requestSpaceX = m_ProtoInfo.Size.x * m_UiItems.Count + PaddingSpace * (m_UiItems.Count - 1);
                    if (requestSpaceX > rect.size.x)    // if there are too many items, spread them closer
                    {
                        float positionLeft = rect.xMin + m_ProtoInfo.Size.x / 2;
                        float positionRight = rect.xMax - m_ProtoInfo.Size.x / 2;
                        float interval = (positionRight - positionLeft) / m_UiItems.Count - 1;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            ((UiControllerBase)m_UiItems[i]).transform.position = new Vector2(positionLeft + interval * i, rect.y);
                        }
                    }
                    else    // else fit the padding space as setting
                    {
                        float positionLeft = rect.x - requestSpaceX / 2 + m_ProtoInfo.Size.x / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            ((UiControllerBase)m_UiItems[i]).transform.position = new Vector2(positionLeft + PaddingSpace * i, rect.y);
                        }
                    }
                    break;
                case EDirection.Vertical:
                    float requestSpaceY = m_ProtoInfo.Size.y * m_UiItems.Count + PaddingSpace * (m_UiItems.Count - 1);
                    if (requestSpaceY > rect.size.y)
                    {
                        float positionBottom = rect.yMin + m_ProtoInfo.Size.y / 2;
                        float positionTop = rect.yMax - m_ProtoInfo.Size.y / 2;
                        float interval = (positionTop - positionBottom) / m_UiItems.Count - 1;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            ((UiControllerBase)m_UiItems[i]).transform.position = new Vector2(rect.x, positionBottom + interval * i);
                        }
                    }
                    else
                    {
                        float positionButtom = rect.y - requestSpaceY / 2 + m_ProtoInfo.Size.y / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            ((UiControllerBase)m_UiItems[i]).transform.position = new Vector2(rect.x, positionButtom + PaddingSpace * i);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Create a list item via <see cref="ItemProto"/>.
        /// </summary>
        public T CreateItem<T>() where T : UiControllerBase, IUiListItem
        {
            var uiController = UiApi.OpenUiController(ItemProto, ClassTypeId<UiControllerBase, T>.Id, this.transform);
            uiController.transform.SetParent(transform);
            uiController.Show();
            m_UiItems.Add((IUiListItem)uiController);
            m_UiItems[m_UiItems.Count - 1].OnIndexChanged(m_UiItems.Count - 1);
            // cache proto info
            if (m_ProtoInfoInited == false)
            {
                var rectTransform = (RectTransform)uiController.transform;
                m_ProtoInfo.Size = rectTransform.rect.size;
                m_ProtoInfoInited = true;
            }
            Refresh();
            return (T)uiController;
        }

        public void RemoveItem(int index)
        {
            ((UiControllerBase)m_UiItems[index]).Close();
            m_UiItems.RemoveAt(index);
            for (int i = index; i < m_UiItems.Count - 1; i++)
            {
                m_UiItems[i].OnIndexChanged(i);
            }
            Refresh();
        }

        public void RemoveItem(IUiListItem item)
        {
            RemoveItem(item.IndexInList);
        }

        public void ClearItems()
        {
            foreach (var item in m_UiItems)
            {
                ((UiControllerBase)item).Close();
            }
            m_UiItems.Clear();
        }
    }

    /// <summary>
    /// Inherit this interface by a <see cref="UiControllerBase{TView}"/> to become <see cref="UiList"/>'s member item.
    /// </summary>
    public interface IUiListItem
    {
        public UiList ParentUiList { get; set; }
        public int IndexInList { get; set; }
        public void OnIndexChanged(int newIndex);
        public void OnListRefresh();
    }
}
