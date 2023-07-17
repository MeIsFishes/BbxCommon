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
        internal List<Component> UiItems = new();

#if UNITY_EDITOR
        [Button("Pre-UiInit")]
        private void PreUiInit()
        {
            var uiItems = GetComponentsInChildren<IBbxUiItem>();
            UiItems.Clear();
            foreach (var item in uiItems)
            {
                UiItems.Add((Component)item);
            }
            foreach (var item in uiItems)
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
