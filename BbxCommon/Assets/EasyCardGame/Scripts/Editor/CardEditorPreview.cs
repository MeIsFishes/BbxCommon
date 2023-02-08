using UnityEngine;
using UnityEditor;

namespace CardGame.Editor {
    public class CardEditorPreview {
        private static Vector3 cardPosition = new Vector3(500, 500, 500);
        private static Quaternion cardRotation = Quaternion.Euler(-90, 0, 180);
        private static Quaternion cameraRotation = Quaternion.Euler(0, 0, 0);
        private static Vector3 cameraPosition = new Vector3(0, 0, 17.5f);

        private static Card currentCard;

        public static void Preview (string cardData) {
            if (currentCard == null) {
                currentCard = Resources.Load<Card>("GameCard");
                currentCard = Object.Instantiate(currentCard);
                currentCard.name = "CardEditorCardPreview";
            }

            currentCard.SetCardData(cardData);

            currentCard.SetPosition(cardPosition);
            currentCard.SetRotation(cardRotation);

            currentCard.cardTooltip.Set(false); // close first.
            currentCard.cardTooltip.Set(true); // then open.

            // open window.
            var scene = (SceneView)EditorWindow.GetWindow(typeof(SceneView));
            scene.Show();
            scene.Focus();
            scene.LookAt(cardPosition + cameraPosition, cameraRotation, 10.5f);
        }

        public static void Clear() {
            if (currentCard != null) {
                Debug.Log(currentCard);
                Object.DestroyImmediate (currentCard.gameObject);
            }
        }
    }
}
