using UnityEngine;

using System.Collections.Generic;

namespace CardGame.Loaders {
    /// <summary>
    /// All playable cards that loaded.
    /// </summary>
    public class PlayableCards : Randomizer<TextAsset> {
        private static PlayableCards _Current;
        public static PlayableCards Current {
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

        private Dictionary<string, int> indexes;

        /// <summary>
        /// Find card by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TextAsset Find(string Id) {
            if (indexes == null) {
                Debug.LogError("[PlayableCards] is not loaded, cannot Pick ()");
                return null;
            }

            if (indexes.TryGetValue(Id, out int value)) {
                return Pick(value);
            }

            return null;
        }


        /// <summary>
        /// Load all cards from data folder.
        /// </summary>
        private static void Load () {
            var cardPool = ScriptableObject.CreateInstance<Pool>();
            cardPool.LoadFolderWithoutInstantiate<TextAsset>("Cards");
            Current = new PlayableCards(cardPool.Count);
            Current.indexes = new Dictionary<string, int>();

            var loadedCards = cardPool.GetAllObjects();
            int length = loadedCards.Length;

            for (int i = 0; i < length; i++) {
                var textAsset = (TextAsset)loadedCards[i].Get();
                Current.AddMember(textAsset, JsonUtility.FromJson<GameData.Cards.BaseCard>(textAsset.text).DropRate);
                Current.indexes.Add(textAsset.name, i);
            }
        }

        public PlayableCards (int length) : base (length){

        }
    }
}
