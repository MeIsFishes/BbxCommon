using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public abstract class UiViewBase : MonoBehaviour
    {
        public bool DefaultOpen = true;

        [NonSerialized]
        public UiControllerBase UiController;
        [SerializeField]
        internal List<IBbxUiItem> UiItems = new List<IBbxUiItem>();

#if UNITY_EDITOR
        [Button("Pre-UiInit")]
        private void PreUiInit()
        {
            var uiItems = GetComponentsInChildren<IBbxUiItem>();
            UiItems.AddArray(uiItems, clear: true);
            foreach (var item in UiItems)
            {
                item.PreInit(this);
                EditorUtility.SetDirty((Component)item);
            }
            EditorUtility.SetDirty(this);
        }
#endif

        public abstract string GetResourcePath();

        public abstract Type GetControllerType();

        public abstract int GetUiGroup();
    }
}
