using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Reflection;

namespace BbxCommon.Ui
{
    public abstract class UiViewBase : MonoBehaviour
    {
        #region Common
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

        [Button("Export as Pre-load")]
        private void ExportAsPreLoadUi()
        {
            UiApi.ExportPreLoadUiController(this);
        }
#endif

        public abstract Type GetControllerType();
        #endregion

        #region ControllerTypeId
        // In generic inheritance, for example, think of the code below:
        // UiMyController : UiControllerBase<UiMyView>
        // UiMyController and UiControllerBase<UiMyView> is not the same class. In some cases, we operate a
        // controller with its base class reference, then we can get its type id by its view, instead of using
        // ClassTypeId<UiControllerBase, TController>, because its view holds its real type.

        private int m_ControllerTypeId;
        private bool m_ControllerTypeIdInited;

        internal int GetControllerTypeId()
        {
            if (m_ControllerTypeIdInited == false)
            {
                var type = typeof(ClassTypeId<,>).MakeGenericType(typeof(UiControllerBase), GetControllerType());
                var method = type.GetMethod("GetId", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                m_ControllerTypeId = (int)method.Invoke(null, null);
                m_ControllerTypeIdInited = true;
            }
            return m_ControllerTypeId;
        }
        #endregion
    }
}
