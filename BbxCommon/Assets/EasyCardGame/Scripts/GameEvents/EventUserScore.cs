using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventUserScore : MonoBehaviour {
        [Serializable]
        public class UnityEventDoubleInt : UnityEvent<int, int> { }

        /// <summary>
        /// User score update. UserId, Score.
        /// </summary>
        public static Action<int, int> OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEventDoubleInt OnUserScore;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += UserScore;
        }

        private void OnDisable() {
            OnTriggered -= UserScore;
        }

        private void UserScore(int user, int score) {
            Debug.Assert(user == 0 || user == 1, "Invalid user, who is this? => " + user, this);

            OnUserScore?.Invoke(user, score);
        }
    }
}


