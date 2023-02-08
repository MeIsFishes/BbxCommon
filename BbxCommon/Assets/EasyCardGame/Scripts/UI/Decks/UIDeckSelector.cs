using UnityEngine;
using UnityEngine.UI;

using CardGame.Loaders;
using CardGame.Textures;

using System.Collections.Generic;

namespace CardGame.UI {
    public class UIDeckSelector : MonoBehaviour {
        [SerializeField] private UIDeckButton activeDeckMenu = default;
        [SerializeField] private UIDeckButton userDeckGame = default;
        [SerializeField] private UIDeckButton opponentDeckGame = default;

        [SerializeField] private UIDeckButton selectableDeckPrefab = default;
        [SerializeField] private Transform decksHolder = default;

        [SerializeField] private Navigation navigation = default;

        [SerializeField] private Renderer bigPictureRenderer = default;
        [SerializeField] private Text headerText = default;
        [SerializeField] private Text descText = default;

        [SerializeField] private GameObject setAsActiveButton = default;

        List<UIDeckButton> members = new List<UIDeckButton>();

        private int activePreview;

        /// <summary>
        /// active deck id, PlayerPrefs variable.
        /// </summary>
        public static string ActiveDeckId {
            get {
                return PlayerPrefs.GetString("_ActiveDeck",
                PlayableDecks.Current.Pick(0).Id);
            }

            set {
                PlayerPrefs.SetString("_ActiveDeck",
                value);
            }
        }

        private void UpdateActiveDeckVisual () {
            Debug.LogFormat("[UIDeckSelector] Active Deck Id: {0}", ActiveDeckId);
            activeDeckMenu.SetDeck(ActiveDeckId);
            PointActive();
        }

        private void Start() {
            // draw decks.
            int count = PlayableDecks.Current.Count;
            for (int i = 0; i < count; i++) {
                var deck = PlayableDecks.Current.Pick(i);

                var newDeck = Instantiate(selectableDeckPrefab, decksHolder);
                newDeck.SetDeck(deck);

                members.Add(newDeck);
            }

            navigation.SetTargetRects(members.ToArray ());

            UpdateActiveDeckVisual();

            Networking.Gateway.OnStartGame += (
                userCards, 
                userDeck, 
                opponentsCards, 
                opponentDeck, 
                isConnected) => {
                    userDeckGame.SetDeck(userDeck);
                    opponentDeckGame.SetDeck(opponentDeck);
                };

            
        }

        private void PointActive () {
            var activeDeck = ActiveDeckId;
            var index = PlayableDecks.Current.FindIndex(activeDeck);
            
            for (int i = 0, length = members.Count; i < length; i++) {
                members[i].SetActive(i == index);
            }
        }

        // set the current selected deck as active.
        public void SetAsActive () {
            var deck = PlayableDecks.Current.Pick(activePreview);

            ActiveDeckId = deck.Id;

            setAsActiveButton.SetActive(false);

            UpdateActiveDeckVisual();
        }

        public void PreviewDeck () {
            activePreview = navigation.CurrentSelected;

            for (int i=0, length = members.Count; i<length; i++) {
                members[i].SetPreview(i == activePreview);
            }

            var deck = PlayableDecks.Current.Pick(activePreview);

            setAsActiveButton.SetActive(deck.Id != ActiveDeckId);

            // load texture.
            if (TextureCollectionReader.Readers["DeckTextures"].Textures.TryGetValue(deck.BigAvatar, out BaseTexture texture)) {
                texture.SetMaterial(Card.ShaderTextureName, bigPictureRenderer);
            }

            headerText.text = deck.Name;
            descText.text = deck.Desc;
        }

        public void UpdateNavigation () {
            // find index of the deck.
            var activeDeck = ActiveDeckId;
            var index = PlayableDecks.Current.FindIndex(activeDeck);

            Debug.LogFormat("[UIDeckSelector] Update Navigation Index {0}", index);

            navigation.Interact(index);

            PreviewDeck();
        }

        public void CloseDeckSelector () {
            Game.Current.ClearGame(false, null);
        }
    }
}
