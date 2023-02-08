using UnityEngine;
using UnityEngine.Events;

namespace CardGame.Input {
    public class EventReturnKey : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private InputActions inputActions;
        [SerializeField] private UnityEvent onReturnKey;
#pragma warning restore CS0649

        private void OnEnable() {
            inputActions.OnReturn += ReturnKey;
        }

        private void OnDisable() {
            inputActions.OnReturn -= ReturnKey;
        }

        private void ReturnKey () {
            onReturnKey?.Invoke();
        }
    }
}
