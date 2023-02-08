using UnityEngine;
using UnityEngine.UI;

using System;

using CardGame.Textures;
using CardGame.Loaders;

using CardGame.GameData.Decks;

namespace CardGame.UI {
    public class UIDeckButton : DefaultUINavigationMember {
        [SerializeField] private Renderer iconRenderer = default;
        [SerializeField] private Text header = default;
        [SerializeField] private GameSettings settings = default;
        [SerializeField] private GameObject activeIcon = default;
        [SerializeField] private GameObject previewIcon = default;

        private Deck deck;

        /// <summary>
        /// Set deck with texture and header.
        /// </summary>
        /// <param name="baseTexture"></param>
        /// <param name="header"></param>
        public void SetDeck (string deckId) {
            var deck = PlayableDecks.Current.Find(deckId);
            if (deck == null) {
                Debug.LogErrorFormat ("[UIDeckButton] Deck Id {0} is not found.", deckId);
                return;
            }

            SetDeck(deck);
        }

        public void SetDeck (Deck deck) {
            this.deck = deck;

            if (TextureCollectionReader.Readers["DeckTextures"].Textures.TryGetValue(deck.Avatar, out BaseTexture texture)) {
                texture.SetMaterial(Card.ShaderTextureName, iconRenderer);
                header.text = deck.Name;
            }
        }

        public override void Interact() {
            base.Interact();

            Debug.LogFormat ("[UIDeckButton] Selected Deck {0}", deck.Id);

            int length = deck.Cards.Length;
            var playableCards = new string[length];
            for (int i=0; i<length; i++) {
                playableCards[i] = PlayableCards.Current.Find(deck.Cards[i]).text;
            }

            Game.Current.ClearGame(true, () => {
                Game.Current.CreateDeck(settings.CardsPerRound, 0, playableCards, () => {
                    Debug.Log("[UIDeckButton] Deck drawed.");
                });
            });
        }

        public void SetActive (bool value) {
            activeIcon.SetActive(value);
        }

        public void SetPreview(bool value) {
            previewIcon.SetActive(value);
        }
    }
}