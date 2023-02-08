using UnityEngine;
using CardGame.Layouts;
using CardGame.Input;
using CardGame.UI;

namespace CardGame.Players {
    public class LocalPlayer : Player {
        public LocalPlayer (Game game, int layoutIndex) : base (game, layoutIndex) {

        }

        public override string[] GetDeck(int deckSize, out string activeDeck) {
            // get deck id
            activeDeck = UIDeckSelector.ActiveDeckId;
            return GetCards(activeDeck, deckSize);
        }

        public override void Clear() {
            base.Clear();

            game.InputActions.OnCancel -= RegisterToUserDeck;
            game.InputActions.OnCancel -= UserCancelDeckSelection;
            myHand.OnCardSelected -= UserSelectedCardFromDeck;
        }

        private Card selectedDeckCard;

        public override void OnTurn() {
            base.OnTurn();

            RegisterToUserDeck();
        }

        private void RegisterToUserDeck() {
            Debug.Log("[LocalPlayer] RegisterToUserDeck ()");
            myHand.OnCardSelected += UserSelectedCardFromDeck;
            game.InputActions.OnCancel -= RegisterToUserDeck;

            myHand.enabled = true;
        }

        private void UnRegisterFromUserDeck() {
            Debug.Log("[LocalPlayer] UnRegisterFromUserDeck ()");
            myHand.OnCardSelected -= UserSelectedCardFromDeck;
        }

        private void UserSelectedCardFromDeck(Card card) {
            Debug.Log("[LocalPlayer] UserSelectedCardFromDeck ()");

            selectedDeckCard = card;

            card.Selected(true, !InputListener.isDragging || !card.IsOrganic);

            UnRegisterFromUserDeck();

            game.InputActions.OnCancel += UserCancelDeckSelection;
            game.InputActions.OnCancel += RegisterToUserDeck;

            game.interactCardSoundClip.Play();

            myHand.GamePadNavigation = false;

            StartInteraction(card);
        }

        private void UserCancelDeckSelection() {
            Debug.Log("[LocalPlayer] UserCancelDeckSelection ()");
            
            if (selectedDeckCard != null) {
                selectedDeckCard.Selected(false);
                selectedDeckCard = null;
            }

            myHand.GamePadNavigation = true;

            game.InputActions.OnCancel -= UserCancelDeckSelection;

            game.interactCardSoundClip.Play();

            myHand.enabled = false; // disable selection.
            myHand.ForceRefresh(() => {
                myHand.enabled = true; // enable selection after reordering the deck.
            });
        }

        #region card interactions
        private void StartInteraction(Card card) {
            Debug.Assert(card.CurrentLayout != null, "How we interacted with this card??, it doesnt have a layout.");

            if (card.CurrentLayout.LayoutType == LayoutType.Deck) {
                if (!card.IsOrganic) {
                    CardWithTarget(true, card, 0, myHand.FindIndex (card));
                } else {
                    CardWithPlacement(card);
                }
            }
        }

        private bool CardWithTarget(bool fromDeck, Card card, int cardLayoutIndex, int cardIndex) {
            Debug.Log("Card with target.");
            var targetLayouts = GetValidTargetLayouts(card);
            Debug.Assert(targetLayouts != null, "Target layouts null for the given card. Is it a valid card?");

            int effectRange = card.baseCard.EffectRange;
            int rangedEffectRange = effectRange;

            if (card.organicAttackerCard != null && card.organicAttackerCard.AttackType == GameData.Cards.AttackTypes.Both) {
                rangedEffectRange = card.organicAttackerCard.RangedEffectRange;
            }

            if (!fromDeck && !CheckTargetLayoutsAvailable(card, targetLayouts)) {
                Debug.Log("No available target cards for aim. Passing.");
                game.UserNoExtraMove();
                return false;
            } else {
                // start aim from cursor.
                Debug.Log("selected card: " + card);
                game.GameCursor.EnableTargetingWithMark(fromDeck,
                    cardLayoutIndex, 
                    card, (fromDeck) ? card.CursorPosition : card.CursorFocusPoint, 
                    targetLayouts, 
                    effectRange,
                    rangedEffectRange,
                    card.IsAttacker, (int targetLayoutIndex, int targetLayoutMemberIndex) => {
                        Debug.Log(targetLayoutIndex);

                        if (targetLayoutIndex == -1) {
                            game.InputActions.OnCancel?.Invoke();
                            game.InputActions.OnCancelUp?.Invoke();

                            Debug.Log("[LocalPlayer] Cancelled.");
                            return;
                        }

                        if (fromDeck) {
                            // this card moved from deck. reset deck.
                            selectedDeckCard.Selected(false);
                            selectedDeckCard = null;
                            game.InputActions.OnCancel -= UserCancelDeckSelection;
                            game.InputActions.OnCancel -= RegisterToUserDeck;
                        }

                        // inform game for our targeting.
                        game.UserTargetedCard(cardLayoutIndex, cardIndex, targetLayoutIndex, targetLayoutMemberIndex);
                }, card);;

                return true;
            }
        }

        private void CardWithPlacement(Card card) {
            var layouts = GetValidPlacementLayouts(card);

            if (InputListener.isDragging) {
                Debug.Log("[LocalPlayer] Card placement with dragging started.");
                game.GameCursor.EnableTargeting(card.CursorPosition, layouts, CursorFeedbackForPlacement);
                game.GameCursor.OnPositionUpdate += CardFollow;
                game.InputActions.OnDragEnd += CancelFollow;
            } else {
                game.GameCursor.EnableTargeting(false, card.CursorPosition, layouts, TargetingTypes.Placement, CursorFeedbackForPlacement);
            }

            void CardFollow(Vector3 pos) {
                card.SetDragging(pos);
            }

            void CancelFollow () {
                game.GameCursor.OnPositionUpdate -= CardFollow;
                game.InputActions.OnDragEnd -= CancelFollow;
            }
        }

        private void CursorFeedbackForPlacement (int targetLayoutIndex, int targetLayoutMemberIndex) {
            Debug.Log("[LocalPlayer] Cursor feedback for placement.");

            if (targetLayoutIndex == -1) {
                game.InputActions.OnCancel?.Invoke();
                game.InputActions.OnCancelUp?.Invoke();

                Debug.Log("[LocalPlayer] Cancelled.");
                return;
            }

            int selectedCardIndex = myHand.FindIndex(selectedDeckCard);
            game.UserPlacedCard(selectedCardIndex, targetLayoutIndex, targetLayoutMemberIndex);

            // disable registration & effects.
            selectedDeckCard.Selected(false);
            selectedDeckCard = null;
            game.InputActions.OnCancel -= UserCancelDeckSelection;
            game.InputActions.OnCancel -= RegisterToUserDeck;
            //

            Debug.Log("Cursor feed back => targetLayoutIndex=>" + targetLayoutIndex + " memberIndex:" + targetLayoutMemberIndex);
        }

        public override void AimFromCard(int cardLayoutIndex, int cardIndex) {
            base.AimFromCard(cardLayoutIndex, cardIndex);

            Card card = GetCardFromLayout(cardLayoutIndex, cardIndex);

            Debug.Assert(card != null, "Card is null. We cant aim from it. TargetlayoutIndex=> " + cardLayoutIndex + ", targetLayoutMemberIndex: " + cardIndex);

            if (!CardWithTarget(false, card, cardLayoutIndex, cardIndex)) {
                //game.UserPass();
            }
        }
        #endregion
    }
}
