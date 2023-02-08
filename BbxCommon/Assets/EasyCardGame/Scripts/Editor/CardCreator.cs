using UnityEditor;
using UnityEngine;
using System.IO;

using CardGame.Editor;
using CardGame.GameData.Cards;

using System;

namespace CardGame.Editor {
    public class CreateCardEditor : EditorWindow {
        static Action onCreated;
        static string cardName;
        static CardInteractionTypes interactionType = CardInteractionTypes.Organic;
        static CardTypes cardType = CardTypes.Attacker;

        public static void Init(Action _onCreated) {
            onCreated = _onCreated;
            cardName = "newCard";

            // open window.
            var window = (CreateCardEditor)GetWindow(typeof(CreateCardEditor));
            window.Show();
        }

        string errorMessage;
        CreateCardEditor myWindow;

        void OnGUI() {
            if (myWindow == null) {
                myWindow = (CreateCardEditor)GetWindow(typeof(CreateCardEditor));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            GUI.skin = EasyCardEditor.GuiSkin;

            GUILayout.Label("Create Card");

            GUILayout.Space(10);
            cardName = EditorGUIHelper.DrawTextField("Card File Name", ref cardName);
            GUILayout.Space(10);

            interactionType = (CardInteractionTypes)EditorGUILayout.EnumPopup("Interaction Type", interactionType);

            if (interactionType == CardInteractionTypes.Organic) {
                GUILayout.Label("Organic cards should be placed\r\nto table before using them.");
            } else {
                GUILayout.Label("Skill cards cannot be placed to table.\r\nOnly playable from hand to target card.");
            }

            GUILayout.Space(20);

            cardType = (CardTypes)EditorGUILayout.EnumPopup("Card Type", cardType);
            GUILayout.Space(40);
            if (GUILayout.Button("Create", GUILayout.Height(40))) {
                var filePath = EasyCardEditor.AssetsPath + "/Cards/" + cardName + ".json";
                if (File.Exists(filePath)) {
                    errorMessage = "File name exists, please change card name.";
                } else {
                    errorMessage = "";

                    BaseCard card = new BaseCard();
                    card.CardInteractionType = interactionType;
                    card.CardType = cardType;
                    card.Name = cardName;
                    File.WriteAllText(filePath, JsonUtility.ToJson(card, true));

                    onCreated?.Invoke();

                    GetWindow(typeof(CreateCardEditor)).Close();
                }
            }

            GUILayout.Label(errorMessage);
        }
    }

}
