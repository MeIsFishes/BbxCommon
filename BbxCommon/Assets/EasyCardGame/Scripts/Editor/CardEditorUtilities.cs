using UnityEngine;
using UnityEditor;

namespace CardGame.Editor {
    public class CardEditorUtilities : ScriptableObject {
        /// <summary>
        /// Creates a texture with the given width, height and color.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Texture2D MakeTex(int width, int height, Color col) {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        //
    }
}