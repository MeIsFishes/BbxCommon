using UnityEngine;
using System.Collections.Generic;

using CardGame.GameData.Decks;

namespace CardGame.Loaders {
    /// <summary>
    /// All playable decks that loaded.
    /// </summary>
    public class PlayableDecks : Randomizer<Deck> {
        private static PlayableDecks _Current;
        private Dictionary<string, int> indexes;

        public static PlayableDecks Current {
            get {
                if (_Current == null) {
                    Load();
                }

                return _Current;
            }

            private set {
                _Current = value;
            }
        }

        public int FindIndex (string Id) {
            if (indexes == null) {
                Debug.LogError("[PlayableDecks] is not loaded, cannot Pick ()");
                return -1;
            }

            if (indexes.TryGetValue(Id, out int value)) {
                return value;
            }

            return -1;
        }

        /// <summary>
        /// Find deck by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Deck Find (string Id) {
            var index = FindIndex(Id);
            if (index != -1) {
                return Pick(index);
            }

            return null;
        }

        /// <summary>
        /// Load all decks from data folder.
        /// </summary>
        private static void Load() {
            Debug.Log("[PlayableDecks] Loading.");
            var deckPool = ScriptableObject.CreateInstance<Pool>();
            deckPool.LoadFolderWithoutInstantiate<TextAsset>("Decks");
            Current = new PlayableDecks(deckPool.Count);
            Current.indexes = new Dictionary<string, int>();

            var loadedDecks = deckPool.GetAllObjects();
            int length = loadedDecks.Length;

            for (int i = 0; i < length; i++) {
                var textAsset = (TextAsset)loadedDecks[i].Get();
                var deck = JsonUtility.FromJson<Deck>(textAsset.text);
                deck.Id = textAsset.name;
                Current.AddMember(deck, 100); // drop rate is 1, for all decks.
                Current.indexes.Add(deck.Id, i);
            }

            Debug.Log("[PlayableDecks] Loaded.");
        }

        public PlayableDecks(int length) : base(length) {

        }
    }
}
