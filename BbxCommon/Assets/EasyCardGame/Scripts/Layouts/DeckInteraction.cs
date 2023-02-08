using UnityEngine;
using CardGame.Animation;
using CardGame.Sounds;

namespace CardGame.Layouts {
    [RequireComponent (typeof (DeckLayout))]
    public class DeckInteraction : MonoBehaviour {
        [HideInInspector] [SerializeField] public DeckLayout TargetLayout;
#pragma warning disable CS0649
        #region interaction animation variables.
        [SerializeField] private AnimationCurve interactedPositionCurve;
        [SerializeField] private AnimationCurve interactedRotationCurve;
        [SerializeField] private AnimationCurve interactedScaleCurve;
        [SerializeField] private AnimationCurve interactedScaleCurveBack;
        [SerializeField] private Vector3 interactedPositionAddition;
        [SerializeField] private Vector3 interactedEulerAngles;
        [SerializeField] private float interactedScaleMultiplier;
        [SerializeField] private float interactedPositionSpeed = 2;
        [SerializeField] private float interactedRotationSpeed = 2;
        [SerializeField] private float interactedScaleSpeed = 2;

        [SerializeField] private SoundClip cardSelect;
        #endregion
#pragma warning restore CS0649
        #region interaction private variables.
        public Card CurrentInteracted {
            private set;
            get;
        }

        private Vector3 currentInterestLastPosition;
        private Quaternion currentInterestLastRotation;
        private Vector3 currentInterestLastScale;
        #endregion

        private void OnValidate() {
            TargetLayout = GetComponent<DeckLayout>();
        }

        public void Interact (Card card, bool instant = false) {
            if (card == CurrentInteracted)
                return;

            if (CurrentInteracted != null) {
                if (!CurrentInteracted.IsSelected && CurrentInteracted.CurrentAnimation != null) {
                    // stop the current animation of current interested.
                    CurrentInteracted.CurrentAnimation.StopWithInstantFinish();
                    CurrentInteracted.CurrentAnimation = null;
                }

                if (!CurrentInteracted.IsSelected) {
                    if (!instant) {
                        var targetCard = CurrentInteracted;

                        // put the current interest back.
                        var animation = new AnimationQuery();
                        var move = new MovementAction(CurrentInteracted, currentInterestLastPosition, interactedPositionSpeed, interactedPositionCurve);
                        var rotate = new RotateAction(CurrentInteracted, currentInterestLastRotation, interactedRotationSpeed, interactedRotationCurve);
                        var scale = new ScaleAction(CurrentInteracted, currentInterestLastScale, interactedScaleSpeed, interactedScaleCurveBack);

                        animation.AddToQuery(move);
                        animation.AddToQuery(rotate);
                        animation.AddToQuery(scale);

                        animation.Start(this, () => {
                            targetCard.CurrentAnimation = null;
                        });
                        CurrentInteracted.CurrentAnimation = animation;
                    } else {
                        CurrentInteracted.SetPosition(currentInterestLastPosition);
                        CurrentInteracted.SetRotation(currentInterestLastRotation);
                        CurrentInteracted.SetScale(currentInterestLastScale);
                    }
                }

                CurrentInteracted.Interested(false);
                CurrentInteracted = null;
            }

            if (card != null) {
                if (!card.IsSelected && card.CurrentAnimation != null) {
                    // stop the current animation of current interested.
                    card.CurrentAnimation.StopWithInstantFinish();
                    card.CurrentAnimation = null;
                }

                if (!card.IsSelected) {
                    var targetCard = card;
                    // pick the new card.
                    currentInterestLastPosition = card.GetPosition();
                    currentInterestLastRotation = card.GetRotation();
                    currentInterestLastScale = card.GetScale();

                    var animation = new AnimationQuery();
                    var move = new MovementAction(card, currentInterestLastPosition + interactedPositionAddition, interactedPositionSpeed, interactedPositionCurve);
                    var rotate = new RotateAction(card, Quaternion.Euler(interactedEulerAngles), interactedRotationSpeed, interactedRotationCurve);
                    var scale = new ScaleAction(card, currentInterestLastScale * interactedScaleMultiplier, interactedScaleSpeed, interactedScaleCurve);

                    animation.AddToQuery(move);
                    animation.AddToQuery(rotate);
                    animation.AddToQuery(scale);

                    cardSelect.Play();

                    animation.Start(this, () => {
                        targetCard.CurrentAnimation = null;
                    });

                    card.CurrentAnimation = animation;
                }

                CurrentInteracted = card;

                card.Interested(true);
            }
        }
    }
}
