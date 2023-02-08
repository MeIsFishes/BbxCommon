using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventGameReady : MonoBehaviour {
        /// <summary>
        /// On user pass, 0 user, 1 opponent.
        /// </summary>
        public static Action OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnGameReady;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += GameReady;
        }

        private void OnDisable() {
            OnTriggered -= GameReady;
        }

        private void GameReady () {
            OnGameReady?.Invoke();
        }
    }
}


