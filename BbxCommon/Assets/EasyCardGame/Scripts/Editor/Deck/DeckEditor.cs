using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System.IO;

using CardGame.GameData.Decks;
using CardGame.Textures;

using System;

namespace CardGame.Editor {
    public class DeckEditor : EditorWindow {
        private static void pathToFileName(string[] path) {
            for (int i = 0, length = path.Length; i < length; i++) {
                path[i] = Path.GetFileNameWithoutExtension(path[i]);
            }
        }

        private static string[] _SkillEffects;
        public static string[] SkillEffects {
            get {
                if (_SkillEffects == null) {
                    _SkillEffects = Directory.GetFiles(EasyCardEditor.AssetsPath + "/SkillEffects/").Where(name => !name.EndsWith(".meta")).ToArray();
                    pathToFileName(_SkillEffects);
                }

                return _SkillEffects;
            }
        }

        private static Deck deck;
        private static List<EasyCardEditor.EditorCard> cards;
        private static Action<string> savingAction;
        private static bool showDeckIdentity = true;

        public static void Init(string header, string data, Action<string> saving) {
            savingAction = saving;

            // load all cards.
            EasyCardEditor.LoadCards();

            deck = JsonUtility.FromJson<Deck>(data);

            ClearCards();

            var deckTextures = TextureCollectionReader.Readers["DeckTextures"];
            deckTextures.Read();
            deckTextures.Textures.TryGetValue(deck.Avatar, out CurrentIcon);
            deckTextures.Textures.TryGetValue(deck.BigAvatar, out CurrentBigIcon);

            // load cards.

            // open window.
            var window = (DeckEditor)GetWindow(typeof(DeckEditor));
            window.titleContent = new GUIContent(string.Format ("Editing deck {0}", header));
            window.minSize = new Vector2(350, 350);
            window.Show();

            Debug.LogFormat ("[DeckEditor] Initialized for {0}", data);

            // set current Texture here TODO
        }

        private static void ClearCards () {
            cards = new List<EasyCardEditor.EditorCard>();
            foreach (var card in deck.Cards) {
                bool found = false;
                for (int i = 0, length = EasyCardEditor.LoadedCards.Length; i < length; i++) {
                    if (EasyCardEditor.LoadedCards[i].CardFileName.Equals(card)) {
                        // loaded card.
                        cards.Add(EasyCardEditor.LoadedCards[i]);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    RemoveCard(card);
                    Debug.LogErrorFormat("[DeckEditor] Card not found {0}", card);
                }
            }
        }

        #region private
        private static Texture2D _cardCover;
        public static Texture2D CardCover {
            get {
                if (_cardCover == null) {
                    _cardCover = Resources.Load<Texture2D>("Editor/cardCover");
                }

                return _cardCover;
            }
        }
        #endregion

        #region private variables
        private Vector2 cardScrollPos;
        private Vector2 lastSize;
        #endregion

        DeckEditor myWindow;

        private static BaseTexture CurrentIcon, CurrentBigIcon;

        private void OnGUI() {
            if (myWindow == null) {
                myWindow = (DeckEditor)GetWindow(typeof(DeckEditor));
            }

            GUI.DrawTexture(new Rect(0, 0, myWindow.position.width, myWindow.position.height), EasyCardEditor.BackgroundTexture);

            DrawCards();
        }

        private void DrawCards() {
            GUI.skin = EasyCardEditor.GuiSkin;

            var allAvatars = TextureCollectionReader.Readers["Avatars"].Textures;

            GUILayout.Label("Deck Editor", EditorStyles.boldLabel);

            showDeckIdentity = EditorGUILayout.Foldout(showDeckIdentity, "Deck Info");

            if (showDeckIdentity) {
                GUILayout.Space(20);
                GUILayout.Label("Unity rich text supported.");
                GUILayout.Label(" You can use <i>ITALIC</i> <b>BOLD</b> <color=orange>colored text</color> etc.");

                if (GUILayout.Button("For rich texts commands, check here.")) {
                    Application.OpenURL("https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html");
                }

                // name and desc.
                deck.Name = EditorGUIHelper.DrawTextField("Name", ref deck.Name);
                GUILayout.Label("Description", EditorStyles.helpBox);
                deck.Desc = EditorGUILayout.TextArea(deck.Desc, GUILayout.Height(50));

                void imageField (string header, ref BaseTexture targetTexture, Rect targetRect, Action<string, BaseTexture> assigner) {
                    // draw card avatar.
                    if (targetTexture != null) {
                        var preview = targetTexture.GetPreview();
                        if (preview != null) {
                            GUI.DrawTexture(targetRect, preview);
                        }
                    }

                    if (GUI.Button(new Rect(targetRect.x, targetRect.y + targetRect.size.y, 60, 25), "change")) {
                        Images.Show(header, "DeckTextures", assigner);
                    }

                    targetRect.position -= Vector2.one * 2;
                    targetRect.size += Vector2.one * 4;
                    GUI.DrawTexture(targetRect, EasyCardEditor.CardCover);
                    //
                }

                var windowSize = myWindow.position.size;
                Rect targetRect = new Rect(windowSize.x - 135, 10, 58, 58);
                imageField("48x48 Small Icon", ref CurrentIcon, targetRect, (id, value) => {
                    deck.Avatar = id;
                    CurrentIcon = value;
                });

                targetRect = new Rect(windowSize.x - 70, 10, 58, 78);

                imageField("220x400 Big Icon", ref CurrentBigIcon, targetRect, (id, value) => {
                    deck.BigAvatar = id;
                    CurrentBigIcon = value;
                });
                //

                if (GUILayout.Button ("Save Deck Info")) {
                    SaveDeck();

                    // refresh deck editor.
                    EasyDeckEditor.Init();
                    myWindow.Focus();

                    AssetDatabase.Refresh();
                }
            }

            /// Storage variable if one of them is gonna be removed.
            int m_cardToRemove = -1;

            float offset = showDeckIdentity ? 260 : 40;

            Vector2 maxSize = myWindow.position.size;
            Vector2 cardSize = new Vector2(75, 120);
            Vector2 drawOffset = new Vector2(10, 50);
            Vector2 position = new Vector2(10, 10);
            Vector2 nameOffset = new Vector2(0, 120);
            Vector2 nameSize = new Vector2(75, 30);
            Vector2 deleteButtonOffset = new Vector2(20, 145);
            Vector2 deleteButtonSize = new Vector2(30, 20);
            Vector2 reverseButtonOffset = new Vector2(0, 145);
            Vector2 forwardButtonOffset = new Vector2(50, 145);
            Vector2 smallButtonSize = new Vector2(20, 20);
            Vector2 textureOffset = new Vector2(10, 10);

            GUI.Label(new Rect(10, offset + position.y, myWindow.position.width, myWindow.position.height - offset), "Cards of the deck");
            GUI.color = Color.gray;
            GUI.Box(new Rect(0, offset + position.y, myWindow.position.width, myWindow.position.height - offset), "");
            GUI.color = Color.white;

            offset += 30;

            cardScrollPos = GUI.BeginScrollView(new Rect(0, offset, myWindow.position.width, myWindow.position.height - offset), cardScrollPos, new Rect(0, 0, lastSize.x + cardSize.x + drawOffset.x, lastSize.y + cardSize.y + drawOffset.y));

            void raisePoint() {
                position.x += cardSize.x + drawOffset.x;
                if (position.x + cardSize.x > maxSize.x) {
                    // jump on y.
                    position.x = 10;
                    position.y += cardSize.y + drawOffset.y;
                }
            }

            // create deck button.
            if (GUI.Button(new Rect(position, cardSize), "Add Card")) {
                // open card list and add.
                myWindow.Close();
                
                CardSelector.Show((card) => {
                    myWindow = (DeckEditor)GetWindow(typeof(DeckEditor));
                    AddCard(card);
                });
            }

            raisePoint();

            for (int i = 0, length = cards.Count; i < length; i++) {
                // draw box.
                if (GUI.Button(new Rect(position, cardSize), "Edit")) {
                    var loadedText = File.ReadAllText(string.Format("{0}/Cards/{1}.json", EasyCardEditor.AssetsPath, cards[i].CardFileName));
                    ShowCardEditor.Init(loadedText, cards[i].Name, SkillEffects);
                }

                // draw box.
                if (cards[i].Texture == null) {
                    if (TextureCollectionReader.Readers["Avatars"].Textures.ContainsKey(cards[i].Avatar)) {
                        // force.
                        cards[i].Texture = TextureCollectionReader.Readers["Avatars"].Textures[cards[i].Avatar].GetPreview();
                    }
                }

                if (cards[i].Texture != null) {
                    GUI.DrawTexture(new Rect(position + textureOffset / 2, cardSize - textureOffset), cards[i].Texture);
                }

                GUI.DrawTexture(new Rect(position - new Vector2(1, 1), cardSize + new Vector2(2f, 2f)), CardCover);

                // draw card name.
                GUI.Label(new Rect(position + nameOffset, nameSize), cards[i].CardFileName);

                if (GUI.Button(new Rect(position + deleteButtonOffset, deleteButtonSize), "Del")) {
                    if (EditorUtility.DisplayDialog("Beware!", "Do you want remove " + cards[i].CardFileName + " from the deck?", "Yes delete remove this card from the deck.", "Cancel")) {
                        m_cardToRemove = i;
                    }
                }

                if (GUI.Button(new Rect(position + reverseButtonOffset, smallButtonSize), "<")) {
                    // move backward.
                    var deckCards = deck.Cards;

                    var step = i - 1;

                    if (i == 0) {
                        step = length - 1;
                    }

                    var reverseCard = deckCards[step];
                    var currentCard = deckCards[i];
                    deckCards[step] = currentCard;
                    deckCards[i] = reverseCard;
                    // switch done.

                    deck.Cards = deckCards;

                    ClearCards();

                    SaveDeck();
                }

                if (GUI.Button(new Rect(position + forwardButtonOffset, smallButtonSize), ">")) {
                    // move forward.
                    var deckCards = deck.Cards;

                    var step = i + 1;

                    if (i == length-1) {
                        step = 0;
                    }

                    var forwardCard = deckCards[step];
                    var currentCard = deckCards[i];
                    deckCards[step] = currentCard;
                    deckCards[i] = forwardCard;
                    // switch done.

                    deck.Cards = deckCards;

                    ClearCards();

                    SaveDeck();
                }

                if (i != length - 1) {
                    raisePoint();
                }
            }

            GUI.EndScrollView();

            lastSize = position;

            if (m_cardToRemove != -1) {
                RemoveCard(cards[m_cardToRemove].CardFileName);
            }
        }

        private static void RemoveCard (string cardFileName) {
            var targetCard = cards.Find(x => x.CardFileName.Equals(cardFileName));
            cards.Remove(targetCard);

            var cardList = deck.Cards.ToList();
            cardList.Remove(cardFileName);
            deck.Cards = cardList.ToArray();

            SaveDeck();
        }

        private static void AddCard(string cardFileName) {
            for (int i = 0, length = EasyCardEditor.LoadedCards.Length; i < length; i++) {
                if (EasyCardEditor.LoadedCards[i].CardFileName.Equals(cardFileName)) {
                    // loaded card.
                    cards.Add(EasyCardEditor.LoadedCards[i]);

                    var cardList = deck.Cards.ToList();
                    cardList.Add (cardFileName);
                    deck.Cards = cardList.ToArray();

                    SaveDeck();

                    break;
                }
            }
        }

        private static void SaveDeck () {
            var textData = JsonUtility.ToJson(deck);
            savingAction?.Invoke (textData);
        }
    }
}
