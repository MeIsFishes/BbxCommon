using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.Events;
using Castle.Core;

namespace BbxCommon.Ui
{
    public class UiOptional : MonoBehaviour, IUiPreInit, IUiInit
    {
        #region Wrapper
        /// <summary>
        /// If you store buttons with names, use this wrapper.
        /// </summary>
        [HideInInspector]
        public NameWpData NameWrapper;
        /// <summary>
        /// If you store buttons with indexes, use this wrapper.
        /// </summary>
        [HideInInspector]
        public IndexWpData IndexWrapper;

        [Serializable]
        public struct NameWpData
        {
            [SerializeField]
            private UiOptional m_Ref;
            public NameWpData(UiOptional obj) { m_Ref = obj; }
            /// <summary>
            /// Toggle an option selected state.
            /// </summary>
            public void ToggleSelected(string name) => m_Ref.ToggleSelectedName(name);
            public void Select(string name) => m_Ref.SelectName(name);
            public void Unselect(string name) => m_Ref.UnselectName(name);
            public void AddOnClickCallback(string name, UnityAction callback) => m_Ref.AddOnClickCallbackName(name, callback);
            public UnityAction<string> OnButtonSelected { get { return m_Ref.OnButtonWithNameSelected; } set { m_Ref.OnButtonWithNameSelected = value; } }
            public UnityAction<string> OnButtonUnselected { get { return m_Ref.OnButtonWithNameUnselected; } set { m_Ref.OnButtonWithNameUnselected = value; } }
            /// <summary>
            /// Once selected buttons changed, pass all selected ones to this function.
            /// </summary>
            public UnityAction<List<string>> OnButtonDirty { get { return m_Ref.OnButtonWithNameDirty; } set { m_Ref.OnButtonWithNameDirty = value; } }
        }

        [Serializable]
        public struct IndexWpData
        {
            [SerializeField]
            private UiOptional m_Ref;
            public IndexWpData(UiOptional obj) { m_Ref = obj; }
            /// <summary>
            /// Toggle an option selected state.
            /// </summary>
            public void ToggleSelected(int index) => m_Ref.ToggleSelectedIndex(index);
            public void Select(int index) => m_Ref.ToggleSelectedIndex(index);
            public void Unselect(int index) => m_Ref.ToggleSelectedIndex(index);
            public void AddOnClickCallback(int index, UnityAction callback) => m_Ref.AddOnClickCallbackIndex(index, callback);
            public UnityAction<int> OnButtonSelected { get { return m_Ref.OnButtonWithIndexSelected; } set { m_Ref.OnButtonWithIndexSelected = value; } }
            public UnityAction<int> OnButtonUnselected { get { return m_Ref.OnButtonWithIndexUnselected; } set { m_Ref.OnButtonWithIndexUnselected = value; } }
            /// <summary>
            /// Once selected buttons changed, pass all selected ones to this function.
            /// </summary>
            public UnityAction<List<int>> OnButtonDirty { get { return m_Ref.OnButtonWithIndexDirty; } set { m_Ref.OnButtonWithIndexDirty = value; } }
        }
        #endregion

        #region Common
        public enum EClickWhenSelected
        {
            Unselect,
            KeepSelected,
        }

        public enum EStoreButtonsWith
        {
            Name,
            Index,
        }

        [FoldoutGroup("Selection")]
        public int SelectLimit = 1;
        [FoldoutGroup("Selection")]
        public EClickWhenSelected ClickWhenSelected;

        [FoldoutGroup("Stored Buttons"), Tooltip("Search and create buttons under this transform, instead of using the current GameObject's.")]
        public Transform TransformOverride;
        [FoldoutGroup("Stored Buttons")]
        public EStoreButtonsWith StoreButtonsWith;
        [FoldoutGroup("Stored Buttons"), Tooltip("If true, the UiOptional will search buttons under the TransformOverride while pre-init.")]
        public bool AutoSearchButtons = true;
        [FoldoutGroup("Stored Buttons"), ShowIf("@StoreButtonsWith == EStoreButtonsWith.Name")]
        public SerializableDic<string, Button> ButtonDic;
        [FoldoutGroup("Stored Buttons"), ShowIf("@StoreButtonsWith == EStoreButtonsWith.Index")]
        public List<Button> ButtonList;

        bool IUiPreInit.OnUiPreInit(UiViewBase uiView)
        {
            NameWrapper = new NameWpData(this);
            IndexWrapper = new IndexWpData(this);
            if (TransformOverride == null)
                TransformOverride = transform;
            OnUiPreInitButton();
            return true;
        }

        void IUiInit.OnUiInit(UiControllerBase uiController)
        {
            OnUiInitSelection();
        }
        #endregion

        #region Selection
        #region Variables
        private Dictionary<string, bool> m_SelectedDic = new();
        private List<bool> m_SelectedList = new();
        private List<string> m_SelectedNames = new();
        private List<int> m_SelectedIndexes = new();
        private Dictionary<string, UnityAction> m_CallbackDic = new();
        private Dictionary<int, UnityAction> m_CallbackList = new();

        public UnityAction<string> OnButtonWithNameSelected;
        public UnityAction<int> OnButtonWithIndexSelected;
        public UnityAction<string> OnButtonWithNameUnselected;
        public UnityAction<int> OnButtonWithIndexUnselected;
        public UnityAction<List<string>> OnButtonWithNameDirty;
        public UnityAction<List<int>> OnButtonWithIndexDirty;
        #endregion

        #region Life Cycle
        private void OnUiInitSelection()
        {
            switch (StoreButtonsWith)
            {
                case EStoreButtonsWith.Name:
                    foreach (var pair in ButtonDic)
                    {
                        UnityAction callback = () =>
                        {
                            OnButtonClickName(pair.Key);
                        };
                        pair.Value.onClick.AddListener(callback);
                        m_CallbackDic[pair.Key] = callback;
                    }
                    break;
                case EStoreButtonsWith.Index:
                    for (int i = 0; i < ButtonList.Count; i++)
                    {
                        UnityAction callback = () =>
                        {
                            OnButtonClickIndex(i);
                        };
                        ButtonList[i].onClick.AddListener(callback);
                        m_CallbackList[i] = callback;
                    }
                    break;
            }
        }
        #endregion

        #region Name
        private void OnButtonClickName(string name)
        {
            if (m_SelectedDic[name] == false)
            {
                SelectName(name);
            }
            else
            {
                switch (ClickWhenSelected)
                {
                    case EClickWhenSelected.Unselect:
                        UnselectName(name);
                        break;
                }
            }
        }

        public void ToggleSelectedName(string name)
        {
            if (m_SelectedDic[name] == false)
                SelectName(name);
            else
                UnselectName(name);
        }

        public void SelectName(string name)
        {
            if (m_SelectedDic[name] == false)
            {
                m_SelectedDic[name] = true;
                OnButtonWithNameSelected?.Invoke(name);

                if (m_SelectedNames.Count >= SelectLimit)
                {
                    UnselectName(m_SelectedNames.GetFront());
                    m_SelectedNames.RemoveFront();
                }
                m_SelectedNames.Add(name);
                
                if (OnButtonWithNameDirty != null)
                {
                    OnButtonWithNameDirty(m_SelectedNames);
                }
            }
        }

        public void UnselectName(string name)
        {
            if (m_SelectedDic[name] == true)
            {
                m_SelectedDic[name] = false;
                OnButtonWithNameUnselected?.Invoke(name);
                m_SelectedNames.Remove(name);
                if (OnButtonWithNameDirty != null)
                {
                    OnButtonWithNameDirty(m_SelectedNames);
                }
            }
        }

        public void AddOnClickCallbackName(string name, UnityAction callback)
        {
            ButtonDic[name].onClick.AddListener(callback);
            m_CallbackDic[name] = callback;
        }
        #endregion

        #region Index
        private void OnButtonClickIndex(int index)
        {
            if (m_SelectedList[index] == false)
            {
                SelectIndex(index);
            }
            else
            {
                switch (ClickWhenSelected)
                {
                    case EClickWhenSelected.Unselect:
                        UnselectIndex(index);
                        break;
                }
            }
        }

        public void ToggleSelectedIndex(int index)
        {
            if (m_SelectedList[index] == false)
                SelectIndex(index);
            else
                UnselectIndex(index);
        }

        public void SelectIndex(int index)
        {
            if (m_SelectedList[index] == false)
            {
                m_SelectedList[index] = true;
                OnButtonWithIndexSelected?.Invoke(index);

                if (m_SelectedIndexes.Count >= SelectLimit)
                {
                    UnselectIndex(m_SelectedIndexes.GetFront());
                    m_SelectedIndexes.RemoveFront();
                }
                m_SelectedIndexes.Add(index);

                if (OnButtonWithIndexDirty != null)
                {
                    OnButtonWithIndexDirty(m_SelectedIndexes);
                }
            }
        }

        public void UnselectIndex(int index)
        {
            if (m_SelectedList[index] == true)
            {
                m_SelectedList[index] = false;
                OnButtonWithIndexUnselected?.Invoke(index);
                m_SelectedIndexes.Remove(index);
                if (OnButtonWithIndexDirty != null)
                {
                    OnButtonWithIndexDirty(m_SelectedIndexes);
                }
            }
        }

        public void AddOnClickCallbackIndex(int index, UnityAction callback)
        {
            ButtonList[index].onClick.AddListener(callback);
            m_CallbackList[index] = callback;
        }
        #endregion
        #endregion

        #region Button
        private void OnUiPreInitButton()
        {
            if (AutoSearchButtons)
                SearchButton();
        }

#if UNITY_EDITOR
        [FoldoutGroup("Stored Buttons"), Button]
        private void SearchButton()
        {
            if (TransformOverride == null)
                TransformOverride = transform;
            switch (StoreButtonsWith)
            {
                case EStoreButtonsWith.Name:
                    {
                        ButtonList.Clear();
                        var deletedKey = SimplePool<List<string>>.Alloc();
                        foreach (var pair in ButtonDic)
                        {
                            if (pair.Value == null)
                                deletedKey.Add(pair.Key);
                        }
                        for (int i = 0; i < deletedKey.Count; i++)
                        {
                            ButtonDic.RemoveFromList(deletedKey[i]);
                        }
                        ButtonDic.ForceInit();
                        var buttons = TransformOverride.GetComponentsInChildren<Button>();
                        for (int i = 0; i < buttons.Length; i++)
                        {
                            var button = buttons[i];
                            if (ButtonDic.ContainsValue(button) == false)
                                ButtonDic.SetToList(button.gameObject.name, button);
                        }
                        ButtonDic.ForceInit();
                    }
                    break;
                case EStoreButtonsWith.Index:
                    {
                        ButtonDic.ClearList();
                        ButtonDic.ForceInit();
                        var deletedIndex = SimplePool<List<int>>.Alloc();
                        for (int i = 0; i < ButtonList.Count; i++)
                        {
                            if (ButtonList[i] == null)
                                deletedIndex.Add(i);
                        }
                        for (int i = deletedIndex.Count - 1; i >= 0; i--)
                        {
                            ButtonList.RemoveAt(deletedIndex[i]);
                        }
                        var buttons = TransformOverride.GetComponentsInChildren<Button>();
                        for (int i = 0; i < buttons?.Length; i++)
                        {
                            var button = buttons[i];
                            if (ButtonList.Contains(button) == false)
                                ButtonList.Add(button);
                        }
                    }
                    break;
            }
        }
#endif
        #endregion
    }
}
