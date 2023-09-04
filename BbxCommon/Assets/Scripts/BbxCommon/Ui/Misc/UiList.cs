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

            public void RemoveItem(int index) => m_Ref.RemoveItem(index);
            public void ClearItems() => m_Ref.ClearItems();
        }
        #endregion

        #region Structs and Variables
        private struct ItemInfo
        {
            public GameObject GameObject;
            public UiControllerBase UiController;
            public Vector3 FromPosition;
            public Vector3 ToPosition;
            public bool IsInTranslation;
            public float TranslationElapsedTime;
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

        [InfoBox("ConstantSlot: Giving numbers of a single line and padding space, set each item to the calculated slot." +
            "\nAreaFit: Spread items evenly on the area space, which identical to the RectTransform.size value.")]
        public EArrangement ArragementType;

        [FoldoutGroup("AreaFit")]
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public EDirection AreaDirection;
        [FoldoutGroup("AreaFit"), LabelText("SlotSize")]
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public Vector2 AreaSlotSize;

        [FoldoutGroup("Translation")]
        [Tooltip("Check this option to make items to do a smooth translation, or items will flicker to target positions.")]
        public bool UseTranslation;
        [FoldoutGroup("Translation"), ShowIf("UseTranslation")]
        public AnimationCurve TranslationCurve;
        [FoldoutGroup("Translation"), ShowIf("UseTranslation")]
        public float TranslationTime;
        #endregion

        #region Life Cycle
        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            Wrapper = new WpData(this);
            return true;
        }

        void IUiInit.OnUiInit(UiControllerBase uiController)
        {
            if (TranslationCurve != null && TranslationCurve.keys.Length > 0)
                m_TranslationCurveTime = TranslationCurve[TranslationCurve.keys.Length - 1].time;
        }

        void IUiOpen.OnUiOpen(UiControllerBase uiController)
        {
            SimplePool.Alloc(out m_InTranslationItemIndexs);
        }

        void IUiShow.OnUiShow(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_UiItems[i].UiController != null)
                    m_UiItems[i].UiController.Show();
                else
                    m_UiItems[i].GameObject.SetActive(true);
            }
        }

        void IUiHide.OnUiHide(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_UiItems[i].UiController != null)
                    m_UiItems[i].UiController.Hide();
                else
                    m_UiItems[i].GameObject.SetActive(false);
            }
        }

        void IUiClose.OnUiClose(UiControllerBase uiController)
        {
            ClearItems();
            m_InTranslationItemIndexs.CollectToPool();
        }

        void IUiDestroy.OnUiDestroy(UiControllerBase uiController)
        {
            for (int i = 0; i < m_UiItems.Count; i++)
            {
                if (m_UiItems[i].UiController != null)
                    m_UiItems[i].UiController.Destroy();
            }
        }

        void IUiUpdate.OnUiUpdate(UiControllerBase uiController, float deltaTime)
        {
            UpdateTranslation(deltaTime);
        }
        #endregion

        #region Item Interfaces
        public T AddItem<T>(int index) where T : UiControllerBase
        {
            var uiController = UiApi.OpenUiController<T>(transform);
            if (uiController == null)
            {
#if UNITY_EDITOR
                Debug.LogError("You are creating a UiController has not been pre-loaded.");
#endif
                return null;
            }
            var itemInfo = new ItemInfo();
            uiController.Show();
            itemInfo.GameObject = uiController.gameObject;
            itemInfo.UiController = uiController;
            itemInfo.GameObject.transform.localPosition = Vector3.zero;   // create at center point
            m_UiItems.Insert(index, itemInfo);
            RefreshLayout();
            return uiController;
        }

        public T AddItem<T>() where T : UiControllerBase
        {
            return AddItem<T>(m_UiItems.Count);
        }

        public void RemoveItem(int index)
        {
            if (m_UiItems[index].UiController != null)
                m_UiItems[index].UiController.Close();
            else
                m_UiItems[index].GameObject.Destroy();
            m_UiItems.RemoveAt(index);
            m_InTranslationItemIndexs.Remove(index);
            RefreshLayout();
        }

        public void ClearItems()
        {
            if (m_UiItems.Count == 0)
                return;
            for (int i = m_UiItems.Count - 1; i >= 0; i--)
            {
                RemoveItem(i);
            }
        }
        #endregion

        #region Layout
        private List<ItemInfo> m_UiItems = new();

        private void RefreshLayout()
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
                SetToPosition(0, rect.position);

            // initialize variables
            float areaSize = 0;
            float slotSize = 0;
            switch (AreaDirection)
            {
                case EDirection.Horizontal:
                    areaSize = rect.width;
                    slotSize = AreaSlotSize.x;
                    break;
                case EDirection.Vertical:
                    areaSize = rect.height;
                    slotSize = AreaSlotSize.y;
                    break;
            }

            // if area size can hold the items' requirement
            var requireSize = slotSize * m_UiItems.Count;
            if (requireSize <= areaSize + 0.01f)
            {
                var startPos = (areaSize - requireSize) * 0.5f;
                switch (AreaDirection)
                {
                    case EDirection.Horizontal:
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetToPosition(i, new Vector2(rect.xMin + startPos + slotSize * (0.5f + i), rect.center.y));
                        }
                        break;
                    case EDirection.Vertical:
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetToPosition(i, new Vector2(rect.center.x, rect.yMin + startPos + slotSize * (0.5f + i)));
                        }
                        break;
                }
            }
            // or respread them
            else
            {
                // keep items not to go beyond the border
                switch (AreaDirection)
                {
                    case EDirection.Horizontal:
                        SetToPosition(0, new Vector2(rect.xMin + slotSize * 0.5f, rect.center.y));
                        SetToPosition(m_UiItems.Count - 1, new Vector2(rect.xMax - slotSize * 0.5f, rect.center.y));
                        break;
                    case EDirection.Vertical:
                        SetToPosition(0, new Vector2(rect.center.x, rect.yMin + slotSize * 0.5f));
                        SetToPosition(m_UiItems.Count - 1, new Vector2(rect.center.x, rect.yMax - slotSize * 0.5f));
                        break;
                }
                // then spread other ones
                if (m_UiItems.Count > 2)
                {
                    var interval = (areaSize - slotSize) / (areaSize - 1);
                    switch (AreaDirection)
                    {
                        case EDirection.Horizontal:
                            for (int i = 1; i < m_UiItems.Count - 1; i++)
                            {
                                SetToPosition(i, new Vector2(rect.xMin + slotSize * 0.5f + interval * i, rect.center.y));
                            }
                            break;
                        case EDirection.Vertical:
                            for (int i = 1; i < m_UiItems.Count - 1; i++)
                            {
                                SetToPosition(i, new Vector2(rect.center.x, rect.yMin + slotSize * 0.5f + interval * i));
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region Translation
        private List<int> m_InTranslationItemIndexs;
        private float m_TranslationCurveTime;

        /// <summary>
        /// Set position translation request, and enable its translation.
        /// </summary>
        private void SetToPosition(int index, Vector2 toPosition)
        {
            if (UseTranslation)
            {
                var item = m_UiItems[index];
                item.FromPosition = m_UiItems[index].GameObject.transform.localPosition;
                item.ToPosition = toPosition;
                if (item.IsInTranslation == false)
                {
                    m_InTranslationItemIndexs.Add(index);
                    item.IsInTranslation = true;
                }
                item.TranslationElapsedTime = 0;
                m_UiItems[index] = item;
            }
            else
            {
                var info = m_UiItems[index];
                info.GameObject.transform.localPosition = toPosition;
            }
        }

        private void UpdateTranslation(float deltaTime)
        {
            var translationEndIndexs = SimplePool<List<int>>.Alloc();
            // translate position
            for (int i = 0; i < m_InTranslationItemIndexs.Count; i++)
            {
                var index = m_InTranslationItemIndexs[i];
                var item = m_UiItems[index];
                item.TranslationElapsedTime += deltaTime;
                if (item.TranslationElapsedTime >= TranslationTime)
                {
                    item.TranslationElapsedTime = TranslationTime;
                    translationEndIndexs.Add(i);
                }
                var timeRatio = item.TranslationElapsedTime / TranslationTime;
                var evaluateTime = timeRatio * m_TranslationCurveTime;
                var deltaVector = (m_UiItems[index].ToPosition - m_UiItems[index].FromPosition) * TranslationCurve.Evaluate(evaluateTime);
                m_UiItems[index].GameObject.transform.localPosition = m_UiItems[index].FromPosition + deltaVector;
                m_UiItems[index] = item;
            }
            // remove finished items
            for (int i = translationEndIndexs.Count - 1; i >= 0; i--)
            {
                var item = m_UiItems[m_InTranslationItemIndexs[translationEndIndexs[i]]];
                item.IsInTranslation = false;
                m_UiItems[m_InTranslationItemIndexs[translationEndIndexs[i]]] = item;
                m_InTranslationItemIndexs.UnorderedRemoveAt(translationEndIndexs[i]);
            }
        }
        #endregion
    }
}
