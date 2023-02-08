using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventUserPlaced : MonoBehaviour {
        /// <summary>
        /// On user placed a card, 0 user, 1 opponent.
        /// </summary>
        public static Action<int> OnTriggered;
#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnLocalUserPlaced;
        [SerializeField] private UnityEvent OnOpponentPlaced;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += UserPlaced;
        }

        private void OnDisable() {
            OnTriggered -= UserPlaced;
        }

        private void UserPlaced(int user) {
            Debug.Assert(user == 0 || user == 1, "Invalid user, who is this? => " + user, this);

            if (user == 0)
                OnLocalUserPlaced?.Invoke();
            else {
                OnOpponentPlaced?.Invoke();
            }
        }
    }
}



