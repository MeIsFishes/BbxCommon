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
        public string ExportPath;

        [Button]
        public void Export()
        {
            var path = ExportPath + "/" + SceneManager.GetActiveScene().name + ".asset";
            var asset = AssetFunc.LoadOrCreateAsset<UiSceneAsset>(path);
            var uiViews = GetComponentsInChildren<UiViewBase>();
            asset.UiObjectDatas.Clear();
            foreach (var uiView in uiViews)
            {
                UiSceneAsset.UiObjectData data = new UiSceneAsset.UiObjectData();
                data.PrefabPath = uiView.GetResourcePath();
                data.UiGroup = uiView.GetUiGroup();
                data.DefaultShow = uiView.DefaultShow;
                data.Position = (uiView.transform as RectTransform).localPosition;
                data.Scale = (uiView.transform as RectTransform).localScale;
                data.Pivot = (uiView.transform as RectTransform).pivot;
                asset.UiObjectDatas.Add(data);
            }
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            Debug.Log("Exported UiSceneAsset to " + path + ".");
        }
    }
}
#endif
