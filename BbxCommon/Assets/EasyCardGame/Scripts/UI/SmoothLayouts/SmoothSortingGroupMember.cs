using UnityEngine;
using CardGame.Animation;

namespace CardGame.UI {
    [RequireComponent (typeof (RectTransform))]
    public class SmoothSortingGroupMember : MonoBehaviour, IMovable {
        [HideInInspector] public RectTransform rect;

        void OnValidate () {
            rect = GetComponent<RectTransform>();
        }

        public Vector3 GetPosition() {
            return rect.anchoredPosition;
        }

        public Quaternion GetRotation() {
            return rect.rotation;
        }

        public void SetPosition(Vector3 targetPosition) {
            rect.anchoredPosition = targetPosition;
        }

        public void SetRotation(Quaternion targetRotation) {
            rect.rotation = targetRotation;
        }
    }
}

