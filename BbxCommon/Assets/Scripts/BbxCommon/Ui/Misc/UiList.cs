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
        [FoldoutGroup("AreaFit")]
        [ShowIf("@ArragementType == EArrangement.AreaFit"), Tooltip("Padding space between two items.")]
        public float PaddingSpace;
        [FoldoutGroup("AreaFit")]
        [ShowIf("@ArragementType == EArrangement.AreaFit")]
        public Vector2 SlotSize;

        [FoldoutGroup("Translation")]
        [Tooltip("Check this option to make items to do a smooth translation, or items will flicker to target positions.")]
        public bool UseTranslation;
        [FoldoutGroup("Translation")]
        public AnimationCurve TranslationCurve;
        [FoldoutGroup("Translation")]
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
            var controller = UiApi.OpenUiController<T>(transform);
            if (controller == null)
            {
#if UNITY_EDITOR
                Debug.LogError("You are creating a UiController has not been pre-loaded.");
#endif
                return null;
            }
            var itemInfo = new ItemInfo();
            itemInfo.GameObject = controller.gameObject;
            itemInfo.UiController = controller;
            m_UiItems.Insert(index, itemInfo);
            return controller;
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
            Refresh();
        }

        public void ClearItems()
        {
            for (int i = m_UiItems.Count; i >= 0; i--)
            {
                RemoveItem(i);
            }
        }
        #endregion

        #region Layout
        private List<ItemInfo> m_UiItems = new();

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
                SetFromToPosition(0, rect.position);

            switch (AreaDirection)
            {
                case EDirection.Horizontal:
                    float requestSpaceX = SlotSize.x * m_UiItems.Count + PaddingSpace * (m_UiItems.Count - 1);
                    if (requestSpaceX > rect.size.x)    // if there are too many items, spread them closer
                    {
                        float positionLeft = rect.xMin + SlotSize.x / 2;
                        float positionRight = rect.xMax - SlotSize.x / 2;
                        float interval = (positionRight - positionLeft) / m_UiItems.Count - 1;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetFromToPosition(i,  new Vector2(positionLeft + interval * i, rect.y));
                        }
                    }
                    else    // else fit the padding space as setting
                    {
                        float positionLeft = rect.x - requestSpaceX / 2 + SlotSize.x / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetFromToPosition(i, new Vector2(positionLeft + PaddingSpace * i, rect.y));
                        }
                    }
                    break;
                case EDirection.Vertical:
                    float requestSpaceY = SlotSize.y * m_UiItems.Count + PaddingSpace * (m_UiItems.Count - 1);
                    if (requestSpaceY > rect.size.y)
                    {
                        float positionBottom = rect.yMin + SlotSize.y / 2;
                        float positionTop = rect.yMax - SlotSize.y / 2;
                        float interval = (positionTop - positionBottom) / m_UiItems.Count - 1;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetFromToPosition(i, new Vector2(rect.x, positionBottom + interval * i));
                        }
                    }
                    else
                    {
                        float positionButtom = rect.y - requestSpaceY / 2 + SlotSize.y / 2;
                        for (int i = 0; i < m_UiItems.Count; i++)
                        {
                            SetFromToPosition(i, new Vector2(rect.x, positionButtom + PaddingSpace * i));
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Translation
        private List<int> m_InTranslationItemIndexs;
        private float m_TranslationCurveTime;

        /// <summary>
        /// Set position translation request, and enable its translation.
        /// </summary>
        private void SetFromToPosition(int index, Vector2 position)
        {
            var info = m_UiItems[index];
            info.FromPosition = m_UiItems[index].GameObject.transform.localPosition;
            info.ToPosition = position;
            if (info.IsInTranslation == false)
            {
                m_InTranslationItemIndexs.Add(index);
                info.IsInTranslation = true;
            }
            info.TranslationElapsedTime = 0;
            m_UiItems[index] = info;
        }

        private void UpdateTranslation(float deltaTime)
        {
            var translationEndIndexs = SimplePool<List<int>>.Alloc();
            // translate position
            for (int i = 0; i < m_InTranslationItemIndexs.Count; i++)
            {
                var item = m_UiItems[i];
                item.TranslationElapsedTime += deltaTime;
                if (item.TranslationElapsedTime >= TranslationTime)
                {
                    item.TranslationElapsedTime = TranslationTime;
                    translationEndIndexs.Add(i);
                }
                var timeRatio = item.TranslationElapsedTime / TranslationTime;
                var evaluateTime = timeRatio * m_TranslationCurveTime;
                var deltaVector = (m_UiItems[i].ToPosition - m_UiItems[i].FromPosition) * TranslationCurve.Evaluate(evaluateTime);
                m_UiItems[i].GameObject.transform.position = m_UiItems[i].FromPosition + deltaVector;
            }

            // remove finished items
            for (int i = translationEndIndexs.Count - 1; i >= 0; i--)
            {
                m_UiItems.UnorderedRemoveAt(translationEndIndexs[i]);
            }
        }
        #endregion
    }
}
