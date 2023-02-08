using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    /// <summary>
    /// User targets a card to hit or heal.
    /// </summary>
    public class EventUserTargetedCard : MonoBehaviour {
        /// <summary>
        /// On user targeted a card, 0 user, 1 opponent.
        /// </summary>
        public static Action<int> OnTriggered;
#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnLocalUserTargetedCard;
        [SerializeField] private UnityEvent OnOpponentTargetedCard;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += UserTargeted;
        }

        private void OnDisable() {
            OnTriggered -= UserTargeted;
        }

        private void UserTargeted(int user) {
            Debug.Assert(user == 0 || user == 1, "Invalid user, who is this? => " + user, this);

            if (user == 0)
                OnLocalUserTargetedCard?.Invoke();
            else {
                OnOpponentTargetedCard?.Invoke();
            }
        }
    }
}



