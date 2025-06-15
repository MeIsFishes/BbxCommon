using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ResourcesDictionaryBuilder
{
    [MenuItem("Tools/Build Resources Dictionary")]
    public static void BuildResourcesDictionary()
    {
        string resourcesPath = "Assets/Resources";
        string outputPath = "Assets/Resources/ResourcesDictionary.json";

        string[] assetPaths = AssetDatabase.FindAssets("", new[] { resourcesPath });
        List<ResourceDictionary> resourceList = new List<ResourceDictionary>();
        HashSet<string> fileNames = new HashSet<string>();

        foreach (string guid in assetPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 跳过文件夹和.meta文件
            if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(DefaultAsset)) continue;
            if (assetPath.EndsWith(".meta")) continue;

            if (assetPath.StartsWith(resourcesPath))
            {
                string relativePath = assetPath.Substring(resourcesPath.Length + 1);
                string fileName = Path.GetFileNameWithoutExtension(relativePath);
                relativePath = Path.ChangeExtension(relativePath, null);

                if (!fileNames.Add(fileName))
                {
                    Debug.LogWarning($"Duplicate resource file name: {fileName}, path: {relativePath}");
                    continue;
                }

                resourceList.Add(new ResourceDictionary { name = fileName, path = relativePath });
            }
        }

        var wrapper = new ResourceDictionaryList { list = resourceList };
        File.WriteAllText(outputPath, JsonUtility.ToJson(wrapper, true));
        Debug.Log($"Resources Dictionary built at: {outputPath}");
    }

    [System.Serializable]
    public class ResourceDictionary
    {
        public string name;
        public string path;
    }

    [System.Serializable]
    public class ResourceDictionaryList
    {
        public List<ResourceDictionary> list;
    }
}
