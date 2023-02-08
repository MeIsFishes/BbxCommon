using UnityEngine;
using UnityEditor;

namespace CardGame.Editor {
    /// <summary>
    /// All avatar textures should be sprite due keep the resolution.
    /// This import post processor will handle that automaticly.
    /// </summary>
    public class CardAvatarImportProcessor : AssetPostprocessor {
        private void OnPreprocessTexture() {
            if (assetPath.Contains("Resources/Avatars")) {
                TextureImporter importer = assetImporter as TextureImporter;

                importer.textureType = TextureImporterType.Sprite;

                Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
                if (asset) {
                    EditorUtility.SetDirty(asset);
                }
            }
        }
    }
}