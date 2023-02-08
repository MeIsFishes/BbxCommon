using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CardGame.Input {
    public class EventTouchSupport : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnSupported;
        [SerializeField] private UnityEvent OnNotSupported;
#pragma warning restore CS0649

        private void OnEnable() {
            if (Touchscreen.current != null) {
                OnSupported?.Invoke();
            } else {
                OnNotSupported?.Invoke();
            }
        }
    }
}
