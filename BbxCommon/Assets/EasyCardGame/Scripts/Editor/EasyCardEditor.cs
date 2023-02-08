#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

using CardGame.Textures;

namespace CardGame.Editor
{
    public class EasyCardEditor : EditorWindow {
        [Serializable]
        public class EditorCard {
            public string CardFileName;
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
                if (!Directory.Exists (path)) {
                    Debug.LogErrorFormat ("[EasyCardEditor] Resources is not found at {0}, you may want to change this path from EasyCardEditor.cs", path);
                }
                return path;
            }
        }

        public static EditorCard[] LoadedCards;

        private static string[] _SkillEffects;
        public static string[] SkillEffects {
            get {
                if (_SkillEffects == null) {
                    _SkillEffects = Directory.GetFiles(AssetsPath + "/SkillEffects/").Where(name => !name.EndsWith(".meta")).ToArray();
                    pathToFileName(_SkillEffects);
                }

                return _SkillEffects;
            }
        }

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

        private static Texture2D _cardCover;
        public static Texture2D CardCover {
            get {
                if (_cardCover == null) {
                    _cardCover = Resources.Load<Texture2D>("Editor/cardCover");
                }

                return _cardCover;
            }
        }
        #endregion

        #region private variables
        private Vector2 cardScrollPos;
        private Vector2 lastSize;
        #endregion

        private static void pathToFileName(string[] path) {
            for (int i = 0, length = path.Length; i < length; i++) {
                path[i] = Path.GetFileNameWithoutExtension(path[i]);
            }
        }

        public static void LoadCards() {
            var cardFiles = Directory.GetFiles (AssetsPath + "/Cards/", "*.json");

            TextureCollectionReader.Readers["Avatars"].Read();

            int length = cardFiles.Length;
            LoadedCards = new EditorCard[length];
            for (int i=0; i<length; i++) {
                LoadedCards[i] = JsonUtility.FromJson<EditorCard>(File.ReadAllText (cardFiles[i]));
                LoadedCards[i].CardFileName = Path.GetFileNameWithoutExtension (cardFiles[i]);

                if (TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey (LoadedCards[i].Avatar)) {
                    LoadedCards[i].Texture = TextureCollectionReader.Readers["Avatars"].Textures[LoadedCards[i].Avatar].GetPreview();
                }
            }
        }

        private static void RemoveCard(string fileName) {
            File.Delete(string.Format ("{0}{1}{2}.json", AssetsPath,"/Cards/",fileName));
            File.Delete(string.Format ("{0}{1}{2}.json.meta", AssetsPath, "/Cards/", fileName));
            LoadCards();
        }

        [MenuItem ("Easy Card Game/Easy Card Editor", false, -1)] 
        public static void Init()
        {
            // load all.
            LoadCards();
            //

            // open window.
            var currentWindow = (EasyCardEditor)GetWindow(typeof(EasyCardEditor), false, "Easy Card Editor");
            currentWindow.Show();
        }

        private EasyCardEditor myWindow;

        private void OnGUI()
        {
            if (myWindow == null) {
                myWindow = (EasyCardEditor)GetWindow(typeof(EasyCardEditor));
            }
             
            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), BackgroundTexture);

            DrawCards();
        }

        private void DrawCards () { 
            GUI.skin = GuiSkin;

            GUILayout.Label("Cards", EditorStyles.boldLabel);

            /// Storage variable if one of them is gonna be removed.
            int m_cardToRemove = -1;

            if (LoadedCards == null)
                Init();

            Vector2 maxSize = myWindow.position.size;
            Vector2 cardSize = new Vector2(75, 120);
            Vector2 drawOffset = new Vector2(10, 50);
            Vector2 position = Vector2.zero;
            Vector2 nameOffset = new Vector2(0, 120);
            Vector2 nameSize = new Vector2(75, 30);
            Vector2 deleteButtonOffset = new Vector2(0, 145);
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

            // create card button.
            if (GUI.Button(new Rect(position, cardSize), "New Card")) {
                CreateCardEditor.Init(Init);
            }

            raisePoint();

            for (int i = 0, length = LoadedCards.Length; i < length; i++)
            {
                // draw box.
                if (GUI.Button (new Rect(position, cardSize), "Edit")) {
                    var loadedText = File.ReadAllText(string.Format ("{0}/Cards/{1}.json", AssetsPath, LoadedCards[i].CardFileName));
                    ShowCardEditor.Init(loadedText, LoadedCards[i].CardFileName, SkillEffects);
                }

                if (LoadedCards[i].Texture == null) {
                    if (TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey (LoadedCards[i].Avatar)) {
                        // force.
                        LoadedCards[i].Texture = TextureCollectionReader.Readers["Avatars"].Textures[LoadedCards[i].Avatar].GetPreview();
                    }
                }

                if (LoadedCards[i].Texture != null) {
                    GUI.DrawTexture(new Rect(position + textureOffset / 2, cardSize - textureOffset), LoadedCards[i].Texture);
                }

                GUI.DrawTexture(new Rect(position - new Vector2 (1,1), cardSize + new Vector2 (2f, 2f)), CardCover);

                // draw card name.
                GUI.Label(new Rect(position + nameOffset, nameSize), LoadedCards[i].CardFileName);

                if (GUI.Button(new Rect(position + deleteButtonOffset, deleteButtonSize), "Delete")) {
                    if (EditorUtility.DisplayDialog("Beware!", "Do you want to delete " + LoadedCards[i].Name + "? This cannot be undone!", "Yes delete this card.", "Cancel")) {
                        m_cardToRemove = i;
                    }
                }

                if (i != length - 1) {
                    raisePoint();
                }
            }

            GUI.EndScrollView();

            lastSize = position;

            if (m_cardToRemove != -1)
            {
                RemoveCard(LoadedCards[m_cardToRemove].CardFileName);
            }
        }
    }

    /// <summary>
    /// Easy functions to create UI elements.
    /// </summary>
    public class EditorGUIHelper
    {
        public static string DrawTextField(string header, ref string variable)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header, EditorStyles.helpBox, GUILayout.Width (header.Length * 20));
            var value = EditorGUILayout.TextField(variable);
            GUILayout.EndHorizontal();

            return value;
        }
    }
}

#endif