using System;

namespace CardGame.GameData.Decks {
    [Serializable]
    public class Deck {
        public string Id;
        /// <summary>
        /// Name of the deck.
        /// </summary>
        public string Name;
        /// <summary>
        /// Description of the deck.
        /// </summary>
        public string Desc;
        /// <summary>
        /// Icon of the deck.
        /// </summary>
        public string Avatar;
        /// <summary>
        /// When user investigating the deck,
        /// the big picture will be presented
        /// </summary>
        public string BigAvatar;
        /// <summary>
        /// List of the cards.
        /// Remember that if one of the card is not found,
        /// It will be ignored, and user will play the game with a missing card.
        /// </summary>
        public string[] Cards;
    }
}
