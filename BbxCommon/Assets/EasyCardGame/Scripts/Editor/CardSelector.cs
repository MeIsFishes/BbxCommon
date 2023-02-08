using UnityEngine;
using UnityEditor;

using CardGame.Textures;

using System;
using System.Collections.Generic;

namespace CardGame.Editor {
    public class CardSelector : EditorWindow {
        private static CardSelector myWindow;
        private static Vector2 scrollPos;
        private static Vector2 lastSize;

        private static Action<string> onSelect;

        public static void Show(Action<string> _onSelect) {
            EasyCardEditor.LoadCards();

            TextureCollectionReader.Readers["Avatars"].Read();

            var window = (CardSelector)GetWindow(typeof(CardSelector));
            window.titleContent = new GUIContent("Select card");
            window.Show();

            onSelect = _onSelect;
        }

        private void OnGUI() {
            if (myWindow == null) {
                myWindow = (CardSelector)GetWindow(typeof(CardSelector));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            DrawCards();
        }

        private void DrawCards() {
            GUI.skin = EasyCardEditor.GuiSkin ;

            GUILayout.Label("Cards", EditorStyles.boldLabel);

            if (EasyCardEditor.LoadedCards == null)
                EasyCardEditor.LoadCards();

            Vector2 maxSize = myWindow.position.size;
            Vector2 cardSize = new Vector2(75, 120);
            Vector2 drawOffset = new Vector2(10, 50);
            Vector2 position = Vector2.zero;
            Vector2 nameOffset = new Vector2(0, 120);
            Vector2 nameSize = new Vector2(75, 45);
            Vector2 textureOffset = new Vector2(10, 10);

            scrollPos = GUI.BeginScrollView(new Rect(0, 0, myWindow.position.width, myWindow.position.height), scrollPos, new Rect(0, 0, lastSize.x + cardSize.x + drawOffset.x, lastSize.y + cardSize.y + drawOffset.y));

            void raisePoint() {
                position.x += cardSize.x + drawOffset.x;
                if (position.x + cardSize.x > maxSize.x) {
                    // jump on y.
                    position.x = 0;
                    position.y += cardSize.y + drawOffset.y;
                }
            }

            for (int i = 0, length = EasyCardEditor.LoadedCards.Length; i < length; i++) {
                // draw box.
                if (GUI.Button(new Rect(position, cardSize), "Edit")) {
                    onSelect?.Invoke(EasyCardEditor.LoadedCards[i].CardFileName);
                    myWindow.Close();
                    return;
                }

                // draw box.
                if (EasyCardEditor.LoadedCards[i].Texture == null) {
                    if (TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey(EasyCardEditor.LoadedCards[i].Avatar)) {
                        // force.
                        EasyCardEditor.LoadedCards[i].Texture = TextureCollectionReader.Readers["Avatars"].Textures[EasyCardEditor.LoadedCards[i].Avatar].GetPreview();
                    }
                }

                if (EasyCardEditor.LoadedCards[i].Texture != null) {
                    GUI.DrawTexture(new Rect(position + textureOffset / 2, cardSize - textureOffset), EasyCardEditor.LoadedCards[i].Texture);
                }

                GUI.DrawTexture(new Rect(position - new Vector2(1, 1), cardSize + new Vector2(2f, 2f)), EasyCardEditor.CardCover);

                // draw card name.
                GUI.Label(new Rect(position + nameOffset, nameSize), EasyCardEditor.LoadedCards[i].CardFileName);

                if (i != length - 1) {
                    raisePoint();
                }
            }

            GUI.color = Color.white;

            GUI.EndScrollView();

            lastSize = position;
        }
    }
}