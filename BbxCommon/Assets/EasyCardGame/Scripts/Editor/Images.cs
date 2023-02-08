using UnityEngine;
using UnityEditor;
using CardGame.Textures;
using System.Linq;
using System;

namespace CardGame.Editor {
    public class Images : EditorWindow {
        private static Images myWindow;
        private static Vector2 scrollPos;
        private static Vector2 lastSize;
        private static Action<string, BaseTexture> onReturn;
        private static string header;

        private static string dataName;
        public static void Show (string _header, string _dataName, Action<string, BaseTexture> _onReturn) {
            TextureCollectionReader.Readers[_dataName].Read();

            dataName = _dataName;
            onReturn = _onReturn;

            header = _header;

            var window = (Images)GetWindow(typeof(Images));
            window.titleContent = new GUIContent("Select an image");
            window.Show();
        }

        private void OnGUI() {
            if (myWindow == null) {
                myWindow = (Images)GetWindow(typeof(Images));
            }

            if (!TextureCollectionReader.Readers[dataName].IsLoaded) {
                TextureCollectionReader.Readers[dataName].Read();
            }


            var allImages = TextureCollectionReader.Readers[dataName].Textures;

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);
            GUILayout.Label(string.Format("Showing textures from Resources/{0}", dataName));
            GUILayout.Label(header);

            GUI.skin = EasyCardEditor.GuiSkin;

            Vector2 maxSize = myWindow.position.size;
            Vector2 cardSize = new Vector2(75, 120);
            Vector2 drawOffset = new Vector2(10, 10);
            Vector2 position = new Vector2 (0, 0);
            Vector2 textureOffset = new Vector2(10, 10);

            scrollPos = GUI.BeginScrollView(new Rect(position.x, position.y + 40, myWindow.position.width - position.x, myWindow.position.height - position.y - 40), scrollPos, new Rect(0, 0, lastSize.x + cardSize.x + drawOffset.x, lastSize.y + cardSize.y + drawOffset.y));

            void raisePoint() {
                position.x += cardSize.x + drawOffset.x;
                if (position.x + cardSize.x > maxSize.x) {
                    // jump on y.
                    position.x = 0;
                    position.y += cardSize.y + drawOffset.y;
                }
            }

            for (int i = 0, length = allImages.Count; i < length; i++) {
                // draw box.
                if (GUI.Button(new Rect(position, cardSize), "")) {
                    myWindow.Close();

                    var element = allImages.ElementAt(i);
                    onReturn?.Invoke(element.Key, element.Value);
                }

                var preview = allImages.ElementAt(i).Value.GetPreview();
                if (preview != null) {
                    GUI.DrawTexture(new Rect(position + textureOffset / 2, new Vector2 (cardSize.x, Mathf.Min (preview.height, cardSize.y)) - textureOffset), allImages.ElementAt(i).Value.GetPreview());
                }
                
                GUI.DrawTexture(new Rect(position - new Vector2(1, 1), cardSize + new Vector2(2f, 2f)), EasyCardEditor.CardCover);

                if (i != length - 1) {
                    raisePoint();
                }
            }

            GUI.EndScrollView();

            lastSize = position;
        }
    }
}