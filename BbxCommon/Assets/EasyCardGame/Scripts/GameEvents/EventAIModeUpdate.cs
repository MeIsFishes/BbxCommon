using UnityEngine;
using System;
using UnityEngine.Events;
using CardGame.Players;

namespace CardGame.GameEvents {
    public class EventAIModeUpdate : MonoBehaviour {
        public static Action<int> OnTriggered;

#pragma warning disable CS0649
        [SerializeField] private GameInputs.UnityIntEvent OnAIModeUpdate;
#pragma warning restore CS0649

        private void OnEnable() {
            OnTriggered += AIModeUpdate;
        }

        private void OnDisable() {
            OnTriggered -= AIModeUpdate;
        }

        private void AIModeUpdate (int aiMode) {
            OnAIModeUpdate?.Invoke(aiMode);
        }

        public void SetAIMode (int index) {
            OnTriggered.Invoke (index);
        }
    }
}


