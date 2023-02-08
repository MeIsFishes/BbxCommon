using CardGame.Layouts;
using CardGame.Animation;
using CardGame.GameEvents;

using UnityEngine;
using System;

namespace CardGame.GameFunctions {
    public class UserPlacedCard : BaseFunction {
        private int layoutMemberIndex;
        private int targetLayoutIndex;
        private int targetMemberIndex;

        /// <summary>
        /// User made his move. It can be called by AI, local user or from network.
        /// </summary>
        /// <param name="layoutMemberIndex">layout child index for selected card (which child of the deck layout?)</param>
        /// <param name="targetLayoutIndex">layout index for target card</param>
        /// <param name="targetMemberIndex">layout child index for target card (which child of target layout</param>
        public UserPlacedCard(int layoutMemberIndex, int targetLayoutIndex, int targetMemberIndex, Game game) : base(game) {
            this.layoutMemberIndex = layoutMemberIndex;
            this.targetLayoutIndex = targetLayoutIndex;
            this.targetMemberIndex = targetMemberIndex;
        }

        public override void Trigger(Action onCompleted) {
            EventUserPlaced.OnTriggered?.Invoke(game.currentTurn);

            game.Decks[game.currentTurn].enabled = false; // no interaction.

            var card = game.Decks[game.currentTurn].Get(layoutMemberIndex);

            TableLayout targetLayout = game.GetTableLayoutByUser(targetLayoutIndex);

            Debug.Log("placing into: " + targetLayoutIndex + " - " + targetLayout);

            Debug.Assert(targetLayout != null, "target layout cannot be null. this move request is invalid.", game);

            if (game.currentTurn == 1) {
                game.ShowCard(card, insert);
            } else {
                result();
            }

            void insert () {
                targetLayout.Insert(game.GameCursor.dummyCard, targetMemberIndex, true);
                targetLayout.Refresh(result);
            }

            void result() {
                Debug.Assert(card.IsOrganic, "Non-organic card in placement function...", game);

                if (card.IsOrganic) {
                    Vector3 placementPosition;
                    Quaternion placementRotation;

                    targetLayout.FindTransformByIndex(targetMemberIndex, out placementPosition, out placementRotation);

                    currentAnimation = new AnimationQuery();
                    currentAnimation.AddToQuery(new MovementAction(card, placementPosition, game.placementAnimationData.PositionUpdateSpeed, game.placementAnimationData.PositionCurve, game.placementAnimationData.HeightCurve));
                    currentAnimation.AddToQuery(new RotateAction(card, placementRotation, game.placementAnimationData.RotationUpdateSpeed, game.placementAnimationData.RotationCurve));
                    currentAnimation.AddToQuery(new ScaleAction(card, Vector3.one, 10, AnimationCurve.Linear(0, 0, 1, 1)));

                    card.Placing(true);

                    game.slideCardSoundClip.Play();

                    currentAnimation.Start(game, () => {
                        currentAnimation = null;
                        // placed.
                        game.Scores.CardPlaced(game.currentTurn);

                        card.Placing(false);
                        card.Placed(true);

                        game.putCardSoundClip.Play();

                        targetLayout.Remove(game.GameCursor.dummyCard, false);

                        targetLayout.Insert(card, targetMemberIndex, true);
                        targetLayout.ReOrder();
                        targetLayout.Refresh(() => {
                            game.Decks[game.currentTurn].ReOrder();
                            game.Decks[game.currentTurn].Refresh(() => {
                                game.Players[game.currentTurn].AimFromCard(1 + targetLayoutIndex, targetLayout.FindIndex(card));
                                onCompleted?.Invoke();
                            });
                        });
                    });
                }
            }
        }
    }
}
