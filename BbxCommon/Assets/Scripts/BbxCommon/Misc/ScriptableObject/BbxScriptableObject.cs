using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace BbxCommon
{
    public abstract class BbxScriptableObject : ScriptableObject
    {
        #region Static
#if UNITY_EDITOR
        public static void ExportAssetPath(BbxScriptableObject asset, string path)
        {
            var soAssets = ResourceApi.LoadOrCreateAssetInResources<ScriptableObjectAssets>(GlobalStaticVariable.ExportScriptableObjectPathInResource);
            string groupName = "GameEngineDefault";
            if (asset.LoadingType == ELoadingType.GroupedByName)
                groupName = asset.GroupName;
            soAssets.SetAsset(groupName, path);
            EditorUtility.SetDirty(soAssets);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(soAssets, out string guid, out long localId);
            AssetDatabase.SaveAssetIfDirty(new GUID(guid));
        }
#endif
        #endregion

        public enum ELoadingType
        {
            [Tooltip("Load the ScriptableObject when game launches.")]
            AutoLoading,
            [Tooltip("Will not be loaded until you request to load the specific group.")]
            GroupedByName,
        }

        [FoldoutGroup("Loading Settings")]
        public ELoadingType LoadingType;
        [FoldoutGroup("Loading Settings"), ShowIf("@LoadingType == ELoadingType.GroupedByName")]
        public string GroupName;

        public void Load()
        {
            OnLoad();
        }

        public void Unload()
        {
            OnUnload();
        }

        protected abstract void OnLoad();
        protected virtual void OnUnload() { }
    }
}
