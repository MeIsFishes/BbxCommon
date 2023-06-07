using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace BbxCommon.Ui
{
    public abstract class UiViewBase : MonoBehaviour
    {
        public bool DefaultShow = true;

        [NonSerialized]
        public UiControllerBase UiController;
        [SerializeField]
        internal List<IBbxUiItem> UiItems = new List<IBbxUiItem>();

#if UNITY_EDITOR
        [Button("Pre-UiInit")]
        private void PreUiInit()
        {
            var uiItems = GetComponentsInChildren<IBbxUiItem>();
            UiItems.Clear();
            UiItems.AddArray(uiItems);
            foreach (var item in UiItems)
            {
                item.PreInit(this);
                EditorUtility.SetDirty((Component)item);
            }
            EditorUtility.SetDirty(this);
        }
#endif

        public abstract Type GetControllerType();
    }
}
