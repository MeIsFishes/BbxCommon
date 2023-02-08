using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventGameOver : MonoBehaviour {
        /// <summary>
        /// Trigger game over, true means aborted.
        /// </summary>
        public static Action<bool> OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnGameFinish;
        [SerializeField] private UnityEvent OnGameAbort;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += GameOver;
        }

        private void OnDisable() {
            OnTriggered -= GameOver;
        }

        private void GameOver(bool value) {
            if (value) {
                OnGameAbort?.Invoke();
            } else {
                OnGameFinish?.Invoke();
            }
        }
    }
}


