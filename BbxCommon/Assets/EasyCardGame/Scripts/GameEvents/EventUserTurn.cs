using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventUserTurn : MonoBehaviour {
        /// <summary>
        /// On user turn, 0 user, 1 opponent.
        /// </summary>
        public static Action<int> OnTriggered;

        [SerializeField] private UnityEvent OnLocalUserTurn = default;
        [SerializeField] private UnityEvent OnOpponentsTurn = default;

        private void OnEnable() {
            OnTriggered += UserTurn;
        }

        private void OnDisable() {
            OnTriggered -= UserTurn;
        }

        private void UserTurn (int user) {
            Debug.Assert(user == 0 || user == 1, "Invalid user, who is this? => " + user, this);

            if (user == 0)
                OnLocalUserTurn?.Invoke();
            else {
                OnOpponentsTurn?.Invoke();
            }
        }
    }
}


