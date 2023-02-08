using UnityEngine;

using CardGame.Layouts;
using CardGame.Loaders;

using System.Collections.Generic;
using System.Linq;

namespace CardGame.Players {
    public abstract class Player {
        protected int layoutIndex;
        protected Game game;

        public Player (Game game, int layoutIndex) {
            this.layoutIndex = layoutIndex;
            this.game = game;
        }

        protected DeckLayout myHand => game.Decks[layoutIndex];
        protected TableLayout[] myLayouts => layoutIndex == 0 ? game.UserTables : game.OpponentTables;
        protected TableLayout[] opponentLayouts => layoutIndex == 0 ? game.OpponentTables : game.UserTables;

        /// <summary>
        /// Find card from pointed layout & index.
        /// </summary>
        /// <param name="cardLayoutIndex">0 = Deck of the player (hand), 1 = ranged layout, 2 = melee layout.</param>
        /// <param name="cardIndex"></param>
        /// <returns></returns>
        protected Card GetCardFromLayout (int cardLayoutIndex, int cardIndex) {
            Card card;

            if (cardLayoutIndex == 0) {
                // get card from deck.
                card = myHand.Get(cardIndex);
            } else {
                // get card from table.
                foreach (var layout in myLayouts) {
                    Debug.Log(layout.name);
                }

                card = myLayouts[cardLayoutIndex - 1].Get(cardIndex);

                Debug.Log("layout: => " + myLayouts[cardLayoutIndex - 1]);
                Debug.Log("found card => " + card);
            }

            return card;
        }

        /// <summary>
        /// Get valid target layouts for aiming from the given card.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        protected TableLayout[] GetValidTargetLayouts (Card card) {
            if (card.IsHealer) {
                if (!card.IsOrganic) {
                    return myLayouts; // skill card can heal both tables.
                } else {
                    if (card.baseCard.EffectBothRows) {
                        return myLayouts;
                    } else {
                        // return same row.
                        for (int i = 0; i < 2; i++) {
                            if (myLayouts[i].FindIndex(card) != -1) {
                                return new TableLayout[1] { myLayouts[i] };
                            }
                        }
                    }
                }
            } else if (card.IsAttacker) {
                if (card.IsRangedAttacker) {
                    return opponentLayouts;
                } else {// melee attacker. return only the melee row.
                    return new TableLayout[1] { opponentLayouts[1] };
                }
            }

            return null;
        }

        /// <summary>
        /// Get valid placement table layouts for the given card.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        protected TableLayout[] GetValidPlacementLayouts (Card card) {
            if (card.IsRangedOnly) {
                return new TableLayout[1] { myLayouts[0] };
            } else if (card.IsHealer || card.IsRangedAttacker) {
                return myLayouts;
            } else {
                return new TableLayout[1] { myLayouts[1] };
            }
        }

        protected bool CheckTargetLayoutsAvailable (Card card, Layout[] layouts) {
            foreach (var layout in layouts) {
                if (layout.IsAnyCardElseInside (card)) {
                    return true;
                }
            }

            return false;
        }

        public virtual void Clear () {

        }

        public virtual void OnTurn() { 
        
        }

        /// <summary>
        /// Player needs to aim to a card from card in cardLayoutIndex with cardIndex.
        /// 0 = Deck of the player (hand), 1 = ranged layout, 2 = melee layout.
        /// </summary>
        /// <param name="cardLayoutIndex">Target layout</param>
        /// <param name="cardIndex"></param>
        public virtual void AimFromCard (int cardLayoutIndex, int cardIndex) { 
            
        }

        /// <summary>
        /// Is this player do have a move to play. Or its game over?
        /// </summary>
        /// <returns></returns>
        public bool HaveAMove () {
            Debug.LogFormat  ("[Player] layoutIndex {0}, capacity {1}", layoutIndex, myHand.Capacity);

            int capacity = myHand.Capacity;

            for (int i = 0; i < capacity; i++) {
                if (myHand.cards[i] != null) {
                    if (myHand.cards[i].IsOrganic) {
                        var placementLayouts = GetValidPlacementLayouts(myHand.cards[i]);

                        int length = placementLayouts.Length;
                        for (int l = 0; l < length; l++) {
                            if (placementLayouts[l].FindAPlace () != -1) {
                                return true;
                            }
                        }
                    } else {
                        var targetLayouts = GetValidTargetLayouts(myHand.cards[i]);

                        int length = targetLayouts.Length;
                        for (int l = 0; l < length; l++) {
                            bool anyCard = targetLayouts[l].IsAnyCardElseInside();
                            if (anyCard) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get the user's cards
        /// </summary>
        /// <param name="deckSize"></param>
        /// <returns></returns>
        public virtual string[] GetDeck (int deckSize, out string activeDeck) {
            var cards = GetRandomDeck(deckSize, out activeDeck);
            return cards;
        }

        /// <summary>
        /// Fills with the cards with the given deckId.
        /// </summary>
        /// <param name="deckId"></param>
        /// <param name="deckSize"></param>
        /// <returns></returns>
        public string[] GetCards (string deckId, int deckSize) {
            var deck = PlayableDecks.Current.Find(deckId);

            return FillDeck(deck, deckSize);
        }

        /// <summary>
        /// Randomly picks a deck.
        /// </summary>
        /// <param name="deckSize"></param>
        /// <returns></returns>
        protected string[] GetRandomDeck (int deckSize, out string activeDeck) {
            // first. find a deck.
            var deck = PlayableDecks.Current.Select();

            Debug.LogFormat("[Player] GetRandomDeck => {0}", deck.Id);

            activeDeck = deck.Id;

            return FillDeck(deck, deckSize);
        }

        private string [] FillDeck (GameData.Decks.Deck deck, int deckSize) {
            Debug.LogFormat("[Player] FillDeck ()=> DeckId {0}, DeckSize {1}", deck.Id, deckSize);
            
            var cards = new List<string>();

            int required = deckSize - deck.Cards.Length;

            Debug.LogFormat("[Player] FillDeck ()=> Required cards to generate {0}", required);

            if (required < 0) {
                cards.AddRange(deck.Cards.Take(deckSize));
            } else {
                cards.AddRange(deck.Cards);

                for (int i = 0; i < required; i++) {
                    var tAsset = PlayableCards.Current.Select();
                    if (tAsset == null) {
                        Debug.LogError("[Player] Card came null from card randomizer.");
                    } else {
                        cards.Add(tAsset.name);
                    }
                }
            }

            Debug.LogFormat("[Player] GetDeck=> {0}", cards.Count);

            List<string> deckData = new List<string>();
            // load cards.
            for (int i = 0, length = cards.Count; i < length; i++) {
                var tAsset = PlayableCards.Current.Find(cards[i]);

                if (tAsset == null) {
                    Debug.LogError("[Player] Card came null from card randomizer.");
                } else {
                    deckData.Add(tAsset.text);
                }
            }

            return deckData.ToArray();
        }
    }
}

