using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Entities.UniversalDelegates;

namespace BbxCommon.Ui
{
    public class UiList : MonoBehaviour, IUiPreInit, IUiInit, IUiOpen, IUiShow, IUiHide, IUiClose, IUiDestroy, IUiUpdate
    {
        #region Wrapper
        [HideInInspector]
        public WpData Wrapper;

        [Serializable]
        public struct WpData
        {
            [SerializeField]
            private UiList m_Ref;

            public WpData(UiList obj) { m_Ref = obj; }

            public T CreateItem<T>() where T : UiControllerBase => m_Ref.CreateItem<T>();
            public void RemoveItem(int index) => m_Ref.RemoveItem(index);
            public void ClearItems() => m_Ref.ClearItems();
        }
        #endregion

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

        [Tooltip("Proto view to create items of the list.")]
        public UiViewBase ItemProto;
        [InfoBox("ConstantSlot: Giving numbers of a single line and padding space, set each item to the calculated slot." +
            "\nAreaFit: Spread items evenly on the area space, which identical to the RectTransform.size value.")]
        public EArrangement ArragementType;
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public EDirection AreaDirection;
        [ShowIf("@ArragementType == EArrangement.AreaFit"), Tooltip("Padding space between two items.")]
        public float PaddingSpace;

        private List<GameObject> m_UiItems = new();
        private List<UiControllerBase> m_Controllers = new();
        private bool m_ProtoInfoInited;
        private ProtoInfo m_ProtoInfo;

        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new WpData(this);
            return true;
        }

        void IUiInit.OnUiInit(UiControllerBase uiController) { }

        void IUiOpen.OnUiOpen(UiControllerBase uiController) { }

        void IUiShow.OnUiShow(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_Controllers[i] != null)
                    m_Controllers[i].Show();
                else
                    m_UiItems[i].SetActive(true);
            }
        }

        void IUiHide.OnUiHide(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_Controllers[i] != null)
                    m_Controllers[i].Hide();
                else
                    m_UiItems[(int)i].SetActive(false);
            }
        }

        void IUiClose.OnUiClose(UiControllerBase uiController)
        {
            ClearItems();
        }

        void IUiDestroy.OnUiDestroy(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_Controllers[i] != null)
                    m_Controllers[i].Destroy();
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
                m_UiItems[0].transform.position = rect.position;

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
                            m_UiItems[i].transform.position = new Vector2(positionLeft + interval * i, rect.y);
                        }
                    }
                    else    // else fit the padding space as setting
                    {
                        float positionLeft = rect.x - requestSpaceX / 2 + m_ProtoInfo.Size.x / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            m_UiItems[i].transform.position = new Vector2(positionLeft + PaddingSpace * i, rect.y);
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
                            m_UiItems[i].transform.position = new Vector2(rect.x, positionBottom + interval * i);
                        }
                    }
                    else
                    {
                        float positionButtom = rect.y - requestSpaceY / 2 + m_ProtoInfo.Size.y / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            m_UiItems[i].transform.position = new Vector2(rect.x, positionButtom + PaddingSpace * i);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Create a list item via <see cref="ItemProto"/>.
        /// </summary>
        public T CreateItem<T>() where T : UiControllerBase
        {
            var uiController = UiApi.OpenUiController(ItemProto, ClassTypeId<UiControllerBase, T>.Id, this.transform);
            uiController.transform.SetParent(transform);
            uiController.Show();
            m_UiItems.Add(uiController.gameObject);
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
            if (m_Controllers[index] != null)
                m_Controllers[index].Close();
            else
                m_UiItems[index].Destroy();
            m_UiItems.RemoveAt(index);
            m_Controllers.RemoveAt(index);
            Refresh();
        }

        public void ClearItems()
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_Controllers[i] != null)
                    m_Controllers[i].Close();
            }
            m_UiItems.Clear();
        }
    }
}
