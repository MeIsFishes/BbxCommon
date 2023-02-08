using UnityEngine;
using UnityEngine.Events;

using System;

namespace CardGame.GameEvents {
    public class EventNewRound : MonoBehaviour {
        [Serializable]
        public class UnityEventDoubleInt : UnityEvent<int, int> { }

        /// <summary>
        /// User score update. UserId, Score.
        /// </summary>
        public static Action<int, int> OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEventDoubleInt OnNewRound;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += NewRound;
        }

        private void OnDisable() {
            OnTriggered -= NewRound;
        }

        private void NewRound(int round, int max) {
            OnNewRound?.Invoke(round, max);
        }
    }
}


