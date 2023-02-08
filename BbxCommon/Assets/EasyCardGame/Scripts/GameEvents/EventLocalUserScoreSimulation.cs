using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventLocalUserScoreSimulation : MonoBehaviour {
        [Serializable]
        public class UnityEventInt : UnityEvent<int> { }
        /// <summary>
        /// Local user score simulation update.
        /// </summary>
        public static Action<int> OnTriggered;

        /// <summary>
        /// Score simulation end.
        /// </summary>
        public static Action OnCleared;

#pragma warning disable CS0649
        [SerializeField] private UnityEventInt OnLocalUserScoreSimulationUpdate;
        [SerializeField] private UnityEvent OnLocalUserScoreSimulationClear;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += ScoreSimulationUpdate;
            OnCleared += ScoreSimulationClear;
        }

        private void OnDisable() {
            OnTriggered -= ScoreSimulationUpdate;
            OnCleared -= ScoreSimulationClear;
        }

        private void ScoreSimulationUpdate (int score) {
            OnLocalUserScoreSimulationUpdate?.Invoke(score);
        }

        private void ScoreSimulationClear () {
            OnLocalUserScoreSimulationClear?.Invoke();
        }
    }
}


