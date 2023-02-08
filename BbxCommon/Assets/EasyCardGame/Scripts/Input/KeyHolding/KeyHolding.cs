using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CardGame.Input {
    public abstract class KeyHolding : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
#pragma warning disable CS0649
        [SerializeField] private Image holdingImage;
        [SerializeField] private float holdSpeed = 1;
        [SerializeField] private AnimationCurve holdingCurve;
        [SerializeField] private UnityEvent onHoldCompleted;
        [SerializeField] protected InputActions inputActions;
#pragma warning restore CS0649

        private float currentHold;
        private bool isHolding;
        protected void StartHold () {
            isHolding = true;
        }

        protected void EndHold() {
            isHolding = false;
            currentHold = 0;
            holdingImage.fillAmount = 0;
        }

        private void Update() {
            if (isHolding) {
                currentHold += Time.deltaTime * holdSpeed;

                holdingImage.fillAmount = holdingCurve.Evaluate(currentHold);

                if (holdingImage.fillAmount >= 1) {
                    // hold done. 
                    currentHold = 0;
                    onHoldCompleted?.Invoke();
                    EndHold();
                }
            }
        }

        public void OnPointerDown (PointerEventData eventData) {
            StartHold();
        }

        public void OnPointerUp (PointerEventData eventData) {
            EndHold();
        }
    }
}

