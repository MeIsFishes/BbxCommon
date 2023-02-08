using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    /// <summary>
    /// Follow cursor and get event when it gets enabled and disabled.
    /// </summary>
    public class EventCursor : MonoBehaviour {
        [Serializable]
        public class UnityVector3Event : UnityEvent<Vector3> {}
        /// <summary>
        /// Cursor gets disabled or enabled with cancellation option.
        /// bool isEnabled, Vector3 startingPosition, bool isCancellationPossible
        /// </summary>
        public static Action<bool, Vector3, bool> Triggered;

        /// <summary>
        /// Cursor moved by input.
        /// </summary>
        public static Action Moved;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnCancellationPossible;
        [SerializeField] private UnityEvent OnCancellationImpossible;

        [SerializeField] private UnityEvent OnCursorEnabled;
        [SerializeField] private UnityEvent OnCursorDisabled;

        [SerializeField] private UnityVector3Event OnCursorStartPosition;
#pragma warning restore CS0649

        private void OnEnable() {
            Triggered += Target;
        }

        private void OnDisable() {
            Triggered -= Target;
        }

        private void Target(bool isEnabled, Vector3 startingPosition, bool isCancellationPossible) {
            if (isEnabled) {
                OnCursorEnabled?.Invoke();
                OnCursorStartPosition?.Invoke(startingPosition);
            } else {
                OnCursorDisabled?.Invoke();
            }

            if (isEnabled) {
                if (isCancellationPossible) {
                    OnCancellationPossible?.Invoke();
                } else {
                    OnCancellationImpossible?.Invoke();
                }
            }
        }
    }
}


