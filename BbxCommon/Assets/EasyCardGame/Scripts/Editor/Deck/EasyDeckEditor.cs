#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

using CardGame.Textures;

namespace CardGame.Editor {
    public class EasyDeckEditor : EditorWindow {
        [Serializable]
        public class EditorDeck {
            public string DeckFileName;
            public string Name;
            public string Avatar;
            public Texture Texture;
        }

        #region static variables
        /// <summary>
        /// Path to resources.
        /// </summary>
        public static string AssetsPath {
            get {
                var path = string.Format("{0}/EasyCardGame/Resources", Application.dataPath);
                if (!Directory.Exists(path)) {
                    Debug.LogErrorFormat("[EasyCardEditor] Resources is not found at {0}, you may want to change this path from EasyCardEditor.cs", path);
                }
                return path;
            }
        }

        private static EditorDeck[] decks;

        private static GUISkin _guiSkin;
        public static GUISkin GuiSkin {
            get {
                if (_guiSkin == null) {
                    _guiSkin = Resources.Load<GUISkin>("Editor/cardEditorGUISkin");
                }

                return _guiSkin;
            }
        }

        private static Texture2D _backgroundTexture;
        public static Texture2D BackgroundTexture {
            get {
                if (_backgroundTexture == null) {
                    _backgroundTexture = CardEditorUtilities.MakeTex(22, 22, new Color(0.1f, 0.1f, 0.15f, 1));
                }

                return _backgroundTexture;
            }
        }
        #endregion

        #region private variables
        private Vector2 cardScrollPos;
        private Vector2 lastSize;
        #endregion

        public static void LoadDecks() {
            var cardFiles = Directory.GetFiles(AssetsPath + "/Decks/", "*.json");

            TextureCollectionReader.ReadAll();

            int length = cardFiles.Length;
            decks = new EditorDeck[length];
            for (int i = 0; i < length; i++) {
                decks[i] = JsonUtility.FromJson<EditorDeck>(File.ReadAllText(cardFiles[i]));
                decks[i].DeckFileName = Path.GetFileNameWithoutExtension(cardFiles[i]);

                if (TextureCollectionReader.Readers["DeckTextures"].Textures.ContainsKey(decks[i].Avatar)) {
                    decks[i].Texture = TextureCollectionReader.Readers["DeckTextures"].Textures[decks[i].Avatar].GetPreview();
                }
            }
        }

        private static void RemoveDeck(string fileName) {
            File.Delete(string.Format("{0}{1}{2}.json", AssetsPath, "/Decks/", fileName));
            File.Delete(string.Format("{0}{1}{2}.json.meta", AssetsPath, "/Decks/", fileName));
            LoadDecks();
        }

        [MenuItem("Easy Card Game/Easy Deck Editor", false, -1)]
        public static void Init() {
            // load all.
            LoadDecks();
            //

            // open window.
            var currentWindow = (EasyDeckEditor)GetWindow(typeof(EasyDeckEditor), false, "Easy Deck Editor");
            currentWindow.Show();
        }

        private EasyDeckEditor myWindow;

        private void OnGUI() {
            if (myWindow == null) {
                myWindow = (EasyDeckEditor)GetWindow(typeof(EasyDeckEditor));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), BackgroundTexture);

            DrawCards();
        }

        private void DrawCards() {
            GUI.skin = GuiSkin;

            GUILayout.Label("Decks", EditorStyles.boldLabel);

            /// Storage variable if one of them is gonna be removed.
            int m_cardToRemove = -1;

            if (decks == null)
                Init();

            Vector2 maxSize = myWindow.position.size;
            Vector2 cardSize = new Vector2(75, 75);
            Vector2 drawOffset = new Vector2(10, 50);
            Vector2 position = Vector2.zero;
            Vector2 nameOffset = new Vector2(0, 75);
            Vector2 nameSize = new Vector2(75, 30);
            Vector2 deleteButtonOffset = new Vector2(0, 100);
            Vector2 deleteButtonSize = new Vector2(75, 20);
            Vector2 textureOffset = new Vector2(10, 10);

            cardScrollPos = GUI.BeginScrollView(new Rect(0, 0, myWindow.position.width, myWindow.position.height), cardScrollPos, new Rect(0, 0, lastSize.x + cardSize.x + drawOffset.x, lastSize.y + cardSize.y + drawOffset.y));

            void raisePoint() {
                position.x += cardSize.x + drawOffset.x;
                if (position.x + cardSize.x > maxSize.x) {
                    // jump on y.
                    position.x = 0;
                    position.y += cardSize.y + drawOffset.y;
                }
            }

            // create deck button.
            if (GUI.Button(new Rect(position, cardSize), "New Deck")) {
                DeckCreator.Init(Init);
            }

            raisePoint();

            for (int i = 0, length = decks.Length; i < length; i++) {
                // draw box.
                if (GUI.Button(new Rect(position, cardSize), "Edit")) {
                    var path = string.Format("{0}/Decks/{1}.json", AssetsPath, decks[i].DeckFileName);
                    var loadedText = File.ReadAllText(path);

                    void Save(string data) {
                        File.WriteAllText(path, data);

                        Debug.LogFormat ("[EasyDeckEditor] Saved {0} successfully.", path);
                    }

                    // open deck editor.
                    DeckEditor.Init(decks[i].DeckFileName, loadedText, Save);
                }

                if (decks[i].Texture == null) {
                    if (TextureCollectionReader.Readers["DeckTextures"].Textures.ContainsKey(decks[i].Avatar)) {
                        // force.
                        decks[i].Texture = TextureCollectionReader.Readers["DeckTextures"].Textures[decks[i].Avatar].GetPreview();
                    }
                }

                if (decks[i].Texture != null) {
                    GUI.DrawTexture(new Rect(position + textureOffset / 2, cardSize - textureOffset), decks[i].Texture);
                }

                // draw card name.
                GUI.Label(new Rect(position + nameOffset, nameSize), decks[i].DeckFileName);

                if (GUI.Button(new Rect(position + deleteButtonOffset, deleteButtonSize), "Delete")) {
                    if (EditorUtility.DisplayDialog("Beware!", "Do you want to delete " + decks[i].DeckFileName + "? This cannot be undone!", "Yes delete this card.", "Cancel")) {
                        m_cardToRemove = i;
                    }
                }

                if (i != length - 1) {
                    raisePoint();
                }
            }

            GUI.EndScrollView();

            lastSize = position;

            if (m_cardToRemove != -1) {
                RemoveDeck(decks[m_cardToRemove].DeckFileName);
            }
        }
    }
}

#endif