using UnityEngine;
using System;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class EventUserPass : MonoBehaviour {
        /// <summary>
        /// On user pass, 0 user, 1 opponent.
        /// </summary>
        public static Action<int> OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private UnityEvent OnLocalUserPass;
        [SerializeField] private UnityEvent OnOpponentsPass;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += UserPass;
        }

        private void OnDisable() {
            OnTriggered -= UserPass;
        }

        private void UserPass(int user) {
            Debug.Assert(user == 0 || user == 1, "Invalid user, who is this? => " + user, this);

            if (user == 0)
                OnLocalUserPass?.Invoke();
            else {
                OnOpponentsPass?.Invoke();
            }
        }
    }
}


