using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
namespace BbxCommon.Ui
{
    /// <summary>
    /// Add this component to a UI root with canvas then click "Export".
    /// UiSceneBase can create UI items via the asset exported.
    /// </summary>
    public class UiSceneExporter : MonoBehaviour
    {
        public List<GameObject> UiGroups = new();
        public string ExportPath;
        [Tooltip("The full name of the UI group enum, for getting members via reflection.")]
        public string FullUiGroupType;

        [SerializeField]
        private List<int> m_UiGroupValue = new();

        [Button]
        public void GenerateUiGroups()
        {
            // get enum
            Type type = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(FullUiGroupType);
                if (type != null && type.FullName == FullUiGroupType)
                    break;
            }
            // generate GameObject
            var values = Enum.GetValues(type);
            for (int i = 0; i < values.Length; i++)
            {
                UiGroups.ModifyCount(i + 1);
                m_UiGroupValue.ModifyCount(i + 1);
                var groupName = Enum.GetName(type, values.GetValue(i));
                if (UiGroups[i] != null)
                    UiGroups[i].gameObject.name = groupName;
                else
                    UiGroups[i] = new GameObject(groupName);
                UiGroups[i].AddMissingComponent<RectTransform>();
                UiGroups[i].transform.SetParent(this.transform);
                ((RectTransform)UiGroups[i].transform).localPosition = Vector3.zero;
                m_UiGroupValue[i] = (int)values.GetValue(i);
            }
            // remove extra members
            while (UiGroups.Count > values.Length)
            {
                UiGroups.RemoveBack();
            }
            Debug.Log("Generating finished!");
        }

        [Button]
        public void ExportUiScene()
        {
            var path = ExportPath + "/" + SceneManager.GetActiveScene().name + ".asset";
            var asset = DataApi.LoadOrCreateAsset<UiSceneAsset>(path);
            asset.UiObjectDatas.Clear();
            for (int i = 0; i < UiGroups.Count; i++)
            {
                var uiViews = UiGroups[i].GetComponentsInChildren<UiViewBase>();
                foreach (var uiView in uiViews)
                {
                    UiSceneAsset.UiObjectData data = new UiSceneAsset.UiObjectData();
                    data.PrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(uiView.gameObject).TryRemoveStart("Assets/Resources/").TryRemoveEnd(".prefab");
                    data.UiGroup = m_UiGroupValue[i];
                    data.DefaultShow = uiView.DefaultShow;
                    data.Position = (uiView.transform as RectTransform).localPosition;
                    data.Scale = (uiView.transform as RectTransform).localScale;
                    data.Pivot = (uiView.transform as RectTransform).pivot;
                    asset.UiObjectDatas.Add(data);
                }
            }
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            Debug.Log("Exported UiSceneAsset to " + path + ".");
        }
    }
}
#endif
