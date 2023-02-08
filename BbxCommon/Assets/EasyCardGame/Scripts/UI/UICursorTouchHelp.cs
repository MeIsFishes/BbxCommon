using UnityEngine;
using CardGame.Animation;
using CardGame.GameEvents;

namespace CardGame.UI {
    public class UICursorTouchHelp : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private UIAnimatedPanel helpPanel;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float secondsToShow = 4;
#pragma warning restore CS0649

        private AnimationQuery waitingForHintAnimation;

        private void OnEnable() {
            EventCursor.Moved += Move;
            EventCursor.Triggered += CursorEnable;
        }

        private void OnDisable() {
            EventCursor.Moved -= Move;
            EventCursor.Triggered -= CursorEnable;
        }

        private void CursorEnable(bool isEnabled, Vector3 position, bool cancellationPossible) {
            if (isEnabled) {
                EnableAtPosition(position);
            } else {
                Disable();
            }
        }

        public void EnableAtPosition (Vector3 position) {
            var screenPos = Camera.main.WorldToScreenPoint(position);
            rectTransform.position = screenPos;

            if (waitingForHintAnimation != null) {
                waitingForHintAnimation.Stop();
            }

            waitingForHintAnimation = new AnimationQuery();
            waitingForHintAnimation.AddToQuery(new TimerAction(secondsToShow));
            waitingForHintAnimation.Start(this, () => {
                helpPanel.Open();
            });

            enabled = true;
        }

        void Disable () {
            enabled = false;

            if (waitingForHintAnimation != null) {
                waitingForHintAnimation.Stop();
                waitingForHintAnimation = null;
            }

            helpPanel.Close();
        }

        private void Move () {
            if (waitingForHintAnimation != null) {
                waitingForHintAnimation.Stop();
                waitingForHintAnimation = null;
            }

            helpPanel.Close();
            enabled = false;
        }
    }
}

