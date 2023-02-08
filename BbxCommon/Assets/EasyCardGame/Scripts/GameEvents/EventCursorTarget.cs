using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    /// <summary>
    /// The cursor found a target, or lost it.
    /// </summary>
    public class EventCursorTarget : MonoBehaviour {
        public static Action<bool> Triggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnTargetFound;
        [SerializeField] private UnityEvent OnTargetLost;
#pragma warning restore CS0649

        private void OnEnable() {
            Triggered += Target;
        }

        private void OnDisable() {
            Triggered -= Target;
        }

        private void Target(bool value) {
            if (value) {
                OnTargetFound?.Invoke();
            } else {
                OnTargetLost?.Invoke();
            }
        }
    }
}


