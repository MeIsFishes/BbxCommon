using System;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon.Ui;
using Sirenix.OdinInspector;
using UnityEditor;

namespace BbxCommon
{
    internal class PreLoadUiData : ScriptableObject
    {
        [Serializable]
        internal struct UiData
        {
            public string UiViewPrefabPath;

            public UiData(string uiViewPrefabPath)
            {
                UiViewPrefabPath = uiViewPrefabPath;
            }
        }

        [SerializeField]
        internal SerializableDic<string, UiData> UiDatas;

        [Button]
        internal void ClearUnusedData()
        {
            var unusedList = new List<string>();
            foreach (var pair in UiDatas)
            {
                if (pair.Value.UiViewPrefabPath.IsNullOrEmpty())
                    unusedList.Add(pair.Key);
            }
            for (int i = 0; i < unusedList.Count; i++)
            {
                UiDatas.Remove(unusedList[i]);
            }
        }

        internal void SetUi(UiViewBase uiView)
        {
#if UNITY_EDITOR
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(uiView.gameObject).TryRemoveStart("Assets/Resources/").TryRemoveEnd(".prefab");
            if (path.IsNullOrEmpty())
                DebugApi.LogError("Export UI failed. You shoul export it from the GameObject in folder, but not from an opened instance!" +
                    " In Unity, a prefab in folder and the opened are different ones!");
            else
                DebugApi.Log("Exported path: " + path);
            UiDatas[uiView.GetControllerType().FullName] = new UiData(path);
#endif
        }

        internal UiViewBase GetUiPrefabBy<TController>() where TController : UiControllerBase
        {
#if UNITY_EDITOR
            if (UiDatas.ContainsKey(typeof(TController).FullName) == false)
                DebugApi.LogError("The UiController " + typeof(TController).FullName +
                    " has not been pre-loaded. You should export it from its UiView by clicking the button on inspector first.");
#endif
            var prefab = Resources.Load<GameObject>(UiDatas[typeof(TController).FullName].UiViewPrefabPath);
            return prefab.GetComponent<UiViewBase>();
        }

        internal UiViewBase GetUiPrefabBy(Type type)
        {
#if UNITY_EDITOR
            if (UiDatas.ContainsKey(type.FullName) == false)
                DebugApi.LogError("The UiController " + type.FullName +
                    " has not been pre-loaded. You should export it from its UiView by clicking the button on inspector first.");
#endif
            var prefab = Resources.Load<GameObject>(UiDatas[type.FullName].UiViewPrefabPath);
            return prefab.GetComponent<UiViewBase>();
        }
    }
}
