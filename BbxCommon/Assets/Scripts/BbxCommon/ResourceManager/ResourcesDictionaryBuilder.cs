using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using BbxCommon;

public class ResourcesDictionaryBuilder
{
    [MenuItem("Tools/Build Resources Dictionary")]
    public static void BuildResourcesDictionary()
    {
        string resourcesPath = "Assets/Resources";
        string outputPath = "Assets/Resources/ResourcesDictionary.json";

        string[] assetPaths = AssetDatabase.FindAssets("", new[] { resourcesPath });
        Dictionary<string, string> resourceDict = new Dictionary<string, string>();
        HashSet<string> fileNames = new HashSet<string>();

        foreach (string guid in assetPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

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

                resourceDict[fileName] = relativePath;
            }
        }

        // 使用JsonApi序列化并写入文件
        var jsonData = JsonApi.Serialize(resourceDict);
        File.WriteAllText(outputPath, jsonData.ToJson());
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
