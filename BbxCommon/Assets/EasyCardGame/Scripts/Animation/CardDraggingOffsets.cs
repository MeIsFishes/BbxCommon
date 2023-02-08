using UnityEngine;

namespace CardGame.Animation {
    [CreateAssetMenu(fileName = "CardDraggingOffsets", menuName = "CardGame/Animation/CardDraggingOffsets", order = 1)]
    public class CardDraggingOffsets : ScriptableObject {
        public Vector3 PositionOffset;
        public Vector3 EulerAngles;
    }
}

