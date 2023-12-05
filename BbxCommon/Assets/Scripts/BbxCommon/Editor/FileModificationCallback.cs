using UnityEngine;
using UnityEditor;

namespace BbxCommon.Editor
{
    public class FileModificationCallback : AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var path in paths)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                if (asset is BbxScriptableObject so)
                {
                    BbxScriptableObject.ExportAssetPath(so, path);
                }
            }
            return paths;
        }

        public static AssetMoveResult OnWillMoveAssets(string sourcePath, string destinationPath)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(sourcePath);
            if (asset is BbxScriptableObject so)
            {
                BbxScriptableObject.ExportAssetPath(so, destinationPath);
            }
            return AssetMoveResult.DidMove;
        }
    }
}
