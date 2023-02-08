using UnityEditor;
using UnityEngine;
using System.IO;

using CardGame.GameData.Decks;

using System;

namespace CardGame.Editor {
    public class DeckCreator : EditorWindow {
        static Action onCreated;
        static string deckName;

        public static void Init(Action _onCreated) {
            onCreated = _onCreated;
            deckName = "newDeck";

            // open window.
            var window = (DeckCreator)GetWindow(typeof(DeckCreator));
            window.Show();
        }

        string errorMessage;
        DeckCreator myWindow;

        void OnGUI() {
            if (myWindow == null) {
                myWindow = (DeckCreator)GetWindow(typeof(DeckCreator));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            GUI.skin = EasyCardEditor.GuiSkin;

            GUILayout.Label("Create Deck");

            GUILayout.Space(10);
            deckName = EditorGUIHelper.DrawTextField("Deck Name", ref deckName);
            GUILayout.Space(10);

            GUILayout.Space(40);
            if (GUILayout.Button("Create", GUILayout.Height(40))) {
                var filePath = EasyDeckEditor.AssetsPath + "/Decks/" + deckName + ".json";
                if (File.Exists(filePath)) {
                    errorMessage = "File name exists, please change card name.";
                } else {
                    errorMessage = "";

                    Deck deck = new Deck();
                    deck.Name = deckName;
                    File.WriteAllText(filePath, JsonUtility.ToJson(deck, true));

                    onCreated?.Invoke();

                    GetWindow(typeof(DeckCreator)).Close();
                }
            }

            GUILayout.Label(errorMessage);
        }
    }

}
