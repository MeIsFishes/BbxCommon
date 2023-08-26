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
        internal List<Component> UiInits = new();
        [SerializeField]
        internal List<Component> UiOpens = new();
        [SerializeField]
        internal List<Component> UiShows = new();
        [SerializeField]
        internal List<Component> UiUpdates = new();
        [SerializeField]
        internal List<Component> UiHides = new();
        [SerializeField]
        internal List<Component> UiCloses = new();
        [SerializeField]
        internal List<Component> UiDestroys = new();

#if UNITY_EDITOR
        [Button("Pre-UiInit")]
        private void PreUiInit()
        {
            var uiPreInitRemoves = GetComponentsInChildren<IUiPreInitRemove>();
            foreach (var item in uiPreInitRemoves)
            {
                if (item.DontRemove == false)
                    DestroyImmediate((Component)item);
            }

            int loop = 0;
            while (true)
            {
                bool end = true;
                var uiPreInits = GetComponentsInChildren<IUiPreInit>();
                foreach (var item in uiPreInits)
                {
                    if (item.OnUiPreInit(this) == false)
                        end = false;
                    EditorUtility.SetDirty((Component)item);
                }
                EditorUtility.SetDirty(this);

                if (end) break;

                loop++;
                if (loop > 8)
                {
                    Debug.LogError("UiPreInit has run too many loops! Is there something wrong?");
                    break;
                }
            }

            var uiInits = GetComponentsInChildren<IUiInit>();
            UiInits.Clear();
            foreach (var item in uiInits)
            {
                UiInits.Add((Component)item);
            }

            var uiOpens = GetComponentsInChildren<IUiOpen>();
            UiOpens.Clear();
            foreach (var item in uiOpens)
            {
                UiOpens.Add((Component)item);
            }

            var uiShows = GetComponentsInChildren<IUiShow>();
            UiShows.Clear();
            foreach (var item in uiShows)
            {
                UiShows.Add((Component)item);
            }

            var uiUpdates = GetComponentsInChildren<IUiUpdate>();
            UiUpdates.Clear();
            foreach (var item in uiUpdates)
            {
                UiUpdates.Add((Component)item);
            }

            var uiHides = GetComponentsInChildren<IUiHide>();
            UiHides.Clear();
            foreach (var item in uiHides)
            {
                UiHides.Add((Component)item);
            }

            var uiCloses = GetComponentsInChildren<IUiClose>();
            UiCloses.Clear();
            foreach (var item in uiCloses)
            {
                UiCloses.Add((Component)item);
            }

            var uiDestroys = GetComponentsInChildren<IUiDestroy>();
            UiDestroys.Clear();
            foreach (var item in uiDestroys)
            {
                UiDestroys.Add((Component)item);
            }
        }
#endif

        public abstract Type GetControllerType();
    }
}
