using UnityEngine;
using CardGame.Loaders;

namespace CardGame.Layouts.Tests {
    public class LayoutTest : MonoBehaviour {
        public Layout targetLayout;
        public Card[] startingCards;
        public Randomizer<TextAsset> cardRandomizer;
        
        // Start is called before the first frame update
        void Start() {
            // create card randomizer.
            var cardPool = ScriptableObject.CreateInstance<Pool>();
            cardPool.LoadFolderWithoutInstantiate<TextAsset>("Cards");
            cardRandomizer = new Randomizer<TextAsset>(cardPool.Count);
            for (int i = 0; i < cardPool.Count; i++) {
                var textAsset = cardPool.GetRandom<TextAsset>();
                cardRandomizer.AddMember(textAsset, JsonUtility.FromJson<GameData.Cards.BaseCard>(textAsset.text).DropRate);
            }
            //

            foreach (Card card in startingCards) {
                card.SetCardData(cardRandomizer.Select().text);
                targetLayout.Add(card);
            }

            targetLayout.Refresh(null, false, true);
        }

        public bool AddNewCard;
        public Card WhichCard;
        public int index;

        // Update is called once per frame
        void Update() {
            if (AddNewCard) {
                AddNewCard = false;
                targetLayout.Insert(WhichCard, index);
                targetLayout.Refresh();
            }
        }
    }
}

