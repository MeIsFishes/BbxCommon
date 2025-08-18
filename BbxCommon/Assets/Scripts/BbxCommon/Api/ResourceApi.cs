using BbxCommon.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BbxCommon
{
    public static class ResourceApi
    {
        #region Internal Variables
        internal static Dictionary<string, List<CsvDataBase>> DataGroupCsvPairs = new();
        #endregion

        #region Resource Manager
        public static ResourceManager.FileInfo GetFile(string key)
        {
            return ResourceManager.GetFirstFile(key);
        }

        public static List<ResourceManager.FileInfo> GetAllFile(string key)
        {
            return ResourceManager.GetFileList(key);
        }

        public static TextAsset LoadTextAsset(string key)
        {
            return ResourceManager.LoadTextAsset(key);
        }

        public static List<TextAsset> LoadTextAssets(string key)
        {
            return ResourceManager.LoadTextAssets(key);
        }
        #endregion

        #region Editor Operation
        public static class EditorOperation
        {
            /// <summary>
            /// "Assets/Resources/MyFolder/MyFile.txt" returns "MyFolder/Myfile".
            /// </summary>
            public static string RelativePathToResourcesPath(string path)
            {
                path = path.TryRemoveStart("Assets/Resources/");
                var dotIndex = path.LastIndexOf('.');
                if (dotIndex != -1)
                    path = path.Substring(0, dotIndex);
                return path;
            }

#if UNITY_EDITOR
            public static void SetDirtyAndSave(Object obj)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssetIfDirty(obj);
            }

            public static TAsset LoadOrCreateAsset<TAsset>(string path) where TAsset : ScriptableObject
            {
                CreateDirectory(path);
                var asset = AssetDatabase.LoadAssetAtPath<TAsset>(path);
                if (asset != null)
                    return asset;
                else
                {
                    if (path.EndsWith(".asset") == false)
                        path += ".asset";
                    asset = ScriptableObject.CreateInstance<TAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                    return asset;
                }
            }

            public static TAsset LoadOrCreateAssetInResources<TAsset>(string path) where TAsset : ScriptableObject
            {
                CreateDirectoryInResources(path);
                if (path.EndsWith(".asset") == false)
                    path += ".asset";
                var asset = AssetDatabase.LoadAssetAtPath<TAsset>("Assets/Resources/" + path);
                if (asset != null)
                    return asset;
                else
                {
                    asset = ScriptableObject.CreateInstance<TAsset>();
                    AssetDatabase.CreateAsset(asset, "Assets/Resources/" + path);
                    return asset;
                }
            }

            public static void CreateDirectory(string path)
            {
                path = Application.dataPath + "/" + path.TryRemoveStart("Assets/");
                FileApi.CreateAbsoluteDirectory(path);
            }

            public static void CreateDirectoryInResources(string path)
            {
                path = Application.dataPath + "/Resources/" + path;
                FileApi.CreateAbsoluteDirectory(path);
            }
#endif
        }
        #endregion
    }
}
