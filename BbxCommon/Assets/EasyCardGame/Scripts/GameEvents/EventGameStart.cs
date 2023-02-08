using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventGameStart : MonoBehaviour {
        public static Action OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnGameStart;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += GameStart;
        }

        private void OnDisable() {
            OnTriggered -= GameStart;
        }

        private void GameStart() {
            OnGameStart?.Invoke();
        }
    }
}


