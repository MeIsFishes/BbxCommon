using UnityEngine;

using CardGame.Layouts;
using CardGame.Input;
using CardGame.Players;
using CardGame.Animation;
using CardGame.UI;
using CardGame.GameEvents;
using CardGame.Effects;
using CardGame.Networking;
using CardGame.GameFunctions;
using CardGame.Sounds;
using CardGame.Loaders;
using CardGame.Textures;

using System;
using System.Linq;

namespace CardGame {
    public class Game : MonoBehaviour {
        public static Game Current { private set; get; }

        public DeckLayout[] Decks = new DeckLayout[2];
        public TableLayout[] UserTables = new TableLayout[2];
        public TableLayout[] OpponentTables = new TableLayout[2];
        public Cursor GameCursor;
        public InputActions InputActions;
        public PlacementAnimationData placementAnimationData;
        public Pool effectsData;
        public Player[] Players;
        public Scores Scores;

        public SoundClip putCardSoundClip;
        public SoundClip slideCardSoundClip;
        public SoundClip interactCardSoundClip;

        private int _currentRound;
        private int currentRound {
            get {
                return _currentRound;
            } 

            set {
                _currentRound = value;
                EventNewRound.OnTriggered?.Invoke(currentRound, gameSettings.RoundCount);
            }
        }

        private string aiDeck;

        public int currentTurn { get; private set; }

#pragma warning disable CS0649
        #region Data files
        [SerializeField] private Pool gameCardsData;
        [SerializeField] private Pool hitEffectsData;
        #endregion

        [SerializeField] private Transform showOpponentCardPoint;
        [SerializeField] private Transform graveyard;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private Material defaultCardMaterial;

        [Tooltip ("Game will create effects, cards etc. to use in the game. This is the parent.")]
        [SerializeField] private Transform gameObjectsHolder;
#pragma warning restore CS0649

        private int graveyardCounter;
        public bool isGameStarted { private set; get; }
        private bool isAIEnabled;

        private int willGameStartWithMaster = -1;

        private GameFunctionQuery gameFunctionQuery;

        private AnimationQuery showCardAnimation;
        private AnimationQuery colorAnimation;

        private bool gameIsNotReady;

        private AIMode aiMode;

        private void Awake () {
            Current = this;

            Scores = new Scores();

            // register to game inputs.
            Gateway.OnStartGame += StartGame;
            Gateway.OnAbortGame += GameOver;
            Gateway.OnPlayWithAIRequest += () => {
                StartGameWithAI();
            };

            Gateway.OnLeftGame += () => {
                GameOver(true);
            };

            Gateway.OnLeftGame += HardClear;

            // we have started to look for a match. Clear game.
            Gateway.OnQuickMatchRequest += () => {
                ClearGame(false, null);
            };

            EventAIModeUpdate.OnTriggered += (int aiMode) => {
                this.aiMode = (AIMode) aiMode;
            };

            Gateway.OnNeedToSendCardData += () => {
                // generate a deck and send.

                var getCards = Players[0].GetDeck(gameSettings.CardsPerRound, out string deckId);

                Gateway.Instance.SendCardDataToNetwork(getCards, deckId);
            };

            // create game function query and register the game events.
            gameFunctionQuery = new GameFunctionQuery();

            Gateway.OnGameTargetData += (a, b, c, d) => {
                gameFunctionQuery.AddToQuery(new UserTargetedCard(a, b, c, d, this));
            };

            Gateway.OnGamePlacementData += (a, b, c) => {
                gameFunctionQuery.AddToQuery(new UserPlacedCard(a, b, c, this));
            };

            Gateway.OnGamePass += () => {
                gameFunctionQuery.AddToQuery(new UserPass(this));
            };

            Gateway.OnGameNoExtraMove += () => {
                gameFunctionQuery.AddToQuery(new UserNoExtraMove(this));
            };

            // start the loop.
            gameFunctionQuery.Play(this);
            //

            Gateway.OnWhoStart = (willMasterStart) => {
                Debug.LogFormat("[Game] OnWhoStart {0}, isGameReady {1}", willMasterStart, !gameIsNotReady);

                if (!gameIsNotReady) {
                    GameIsStartingWith(willMasterStart);
                } else {
                    willGameStartWithMaster = willMasterStart ? 0 : 1;
                }
            };

            effectsData.LoadFolder<SkillEffect>("SkillEffects", 20, gameObjectsHolder);
            gameCardsData.LoadSingleObject<Card>("GameCard", gameSettings.GameCardPoolSize, gameObjectsHolder);
            hitEffectsData.LoadSingleObject<HitEffect>("HitEffect", 10, gameObjectsHolder);

            TextureCollectionReader.ReadAll();

            Debug.Log("[Game] Loaded");

            Players = new Player[2];
            Players[0] = new LocalPlayer(this, 0);
        }

        private void Start() {
            ClearGame(false, () => {
                EventGameReady.OnTriggered?.Invoke();
                Debug.Log("[Game] Game is ready.");
            });
        }

        private void GameIsStartingWith (bool willMasterStart) {
            Debug.LogFormat("[Game] GameIsStartingWith {0}", willMasterStart);

            if (Gateway.Instance.AreWeLobbyMaster) {
                if (willMasterStart) {
                    UserTurn(0);
                } else {
                    UserTurn(1);
                }
            } else {
                if (willMasterStart) {
                    UserTurn(1);
                } else {
                    UserTurn(0);
                }
            }

            willGameStartWithMaster = -1;
        }

        /// <summary>
        /// Prepares the game before any game starts.
        /// </summary>
        private void PrepareGame () {
            ClearGame(true, null); // clear first.

            Scores.ClearScores();

            Decks[1].Close();

            // Opponents deck is never became interactable.
            Decks[1].enabled = false;

            for (int i = 0; i < 2; i++) {
                Decks[i].enabled = false;
            }

            for (int i = 0; i < 2; i++) {
                UserTables[i].enabled = true;
                OpponentTables[i].enabled = true;
            }

            for (int i = 0; i < 2; i++) {
                EventUserScore.OnTriggered?.Invoke(i, 0);
            }

            graveyardCounter = 0;

            currentRound = 1;

            isGameStarted = true;
        }

        /// <summary>
        /// Create deck.
        /// </summary>
        /// <param name="deckSize"></param>
        /// <param name="index"></param>
        /// <param name="cards"></param>
        /// <param name="onCompleted"></param>
        public void CreateDeck (int deckSize, int index, string[] cards, Action onCompleted) {
            Debug.Log("[Game] Create Deck => " + deckSize);

            if (cards == null) {
                Debug.LogError("[Game] Given cards are null");
                return;
            }

            if (deckSize < cards.Length) {
                cards = cards.ToList().Take(deckSize).ToArray();
                Debug.LogFormat("[Game] Cards length {0} is longer than {1}, cutting", cards.Length, deckSize);
            }

            for (int i = 0, length = cards.Length; i < length; i++) {
                // card pool has 1 object pooler. So GetRandom is okay.
                var card = gameCardsData.GetRandom<Card>();

                card.SetCardData(cards[i]);

                card.OnToGraveyard = ResurrectAtGraveyard;

                card.OnCardHit = (int user, int amount) => {
                    Scores.CardHit(user, amount);
                };

                card.OnCardKilled = (int user, int startingHealth) => {
                    Scores.CardKilled(user, startingHealth);
                };

                card.UserId = index;

                Decks[index].Add(card);
            }

            // start fancy mode.
            Decks[index].Refresh(onCompleted, false, true);
        }

        /// <summary>
        /// Creates a new card game against AI.
        /// </summary>
        private void StartGameWithAI(bool forced = false) {
            if (isGameStarted && !forced) {
                Debug.Log("[Game] StartGameWithAI => The game is already started.");
                return;
            }

            Players[1] = new AIPlayer(this, 1, aiMode);
            // create local user's cards.
            var localUserCards = Players[0].GetDeck(gameSettings.CardsPerRound, out string localUserDeck);
            Gateway.Instance.SendCardDataLocalGame(localUserCards, localUserDeck, true);
            // create AI's cards

            string[] opponentCards;
            string opponentDeck;
            if (isGameStarted) {
                opponentCards = Players[1].GetCards(aiDeck, gameSettings.CardsPerRound);
                opponentDeck = aiDeck;
            } else {
                opponentCards = Players[1].GetDeck (gameSettings.CardsPerRound, out opponentDeck);
                aiDeck = opponentDeck;
            }

            Gateway.Instance.SendCardDataLocalGame(opponentCards, opponentDeck, false);
        }

        /// <summary>
        /// Start a game with 2 players. No AI.
        /// </summary>
        /// <param name="userDeck"></param>
        /// <param name="opponentsDeck"></param>
        private void StartGame (string[] userDeck, string userDeckId, string[] opponentsDeck, string opponentsDeckId, bool withAI) {
            gameIsNotReady = true;

            void triggerGame () {
                gameIsNotReady = false;

                if (willGameStartWithMaster != -1) {
                    // whoStarts event already came. so start the game.
                    GameIsStartingWith(willGameStartWithMaster == 0);
                }
            }

            void createDecks(Action onCompleted) {
                bool[] isCompleted = new bool[2];

                CreateDeck(gameSettings.CardsPerRound, 0, userDeck, () => { 
                    isCompleted[0] = true;
                    checkCompletion();
                });
                CreateDeck(gameSettings.CardsPerRound, 1, opponentsDeck, () => { 
                    isCompleted[1] = true;
                    checkCompletion();
                });

                void checkCompletion () {
                    for (int i=0; i<2; i++) {
                        if (!isCompleted [i]) {
                            break;
                        }

                        if (i == 1) {
                            onCompleted?.Invoke();
                        }
                    }
                }
            }

            if (isGameStarted) {
                // already started.
                if (currentRound < gameSettings.RoundCount) {
                    // clear decks.
                    for (int i=0; i<2;i ++) {
                        Decks[i].Clear(true);
                    }
                    
                    RefreshGraveyard(false, () => {
                        createDecks(() => {
                            RefreshGraveyard(false, null);
                            currentRound++;
                            triggerGame();
                        });
                    });

                    Debug.Log("[Round] Round increase");
                } else {
                    Debug.LogError("[Game] Game is already started and rounds completed. But called start game function. First you should stop the game.");
                }
           
                return;
            }

            if (userDeck == null ||opponentsDeck == null) {
                Debug.LogError ("[Game] One of the given decks are null, leaving game.");
                Gateway.Instance.LeaveGame();
                return;
            }

            isAIEnabled = withAI;

            PrepareGame();

            if (withAI) {
                Players[1] = new AIPlayer(this, 1, aiMode);
            } else {
                Players[1] = new NetworkedPlayer(this, 1);
            }

            EventGameStart.OnTriggered?.Invoke();

            createDecks( () => {
                RefreshGraveyard(false, () => {
                    Debug.Log("[Game] New game is prepared.");
                    triggerGame();
                });
            });
        }

        #region user game functions
        public void UserPlacedCard(int layoutMemberIndex, int targetLayoutIndex, int targetMemberIndex) {
            Gateway.Instance.SendPlacementData(layoutMemberIndex, targetLayoutIndex, targetMemberIndex);
        }

        public void UserTargetedCard(int cardLayoutIndex, int cardIndex, int targetLayoutIndex, int targetCardIndex) {
            Gateway.Instance.SendTargetData(cardLayoutIndex, cardIndex, targetLayoutIndex, targetCardIndex);
        }

        public void UserPass() {
            Gateway.Instance.SendPass();
        }

        public void UserNoExtraMove() {
            Gateway.Instance.SendNoExtraMove();
        }
        #endregion

        /// <summary>
        /// A card is destroyed. It will be resurrected at the graveyard.
        /// </summary>
        /// <param name="card"></param>
        private void ResurrectAtGraveyard (Card card, bool isDead, Layout layout) {
            card.OnToGraveyard = null;

            if (layout != null) {
                layout.Remove(card);

                if (isDead) {
                    layout.Refresh(null, false);
                }
            }

            Vector3 targetPosition = graveyard.position;
            Quaternion targetRotation = graveyard.rotation;
            var up = graveyard.up * graveyardCounter * placementAnimationData.GraveyardCardDistance;

            card.SetPosition(targetPosition + up);
            card.SetRotation(targetRotation);
            card.SetScale(Vector3.one);

            colorAnimation = new AnimationQuery();
            colorAnimation.AddToQuery(new ColorAction(card, Card.ShaderMainColor, placementAnimationData.GraveyardResurrectUpdateSpeed, placementAnimationData.GraveyardResurrectGradient, placementAnimationData.GraveyardResurrectIntensity));
            colorAnimation.Start(this, () => {
                card.SetMaterial(defaultCardMaterial);
            });

            graveyardCounter++;
        }

        private void RefreshGraveyard (bool instant, Action onCompleted) {
            Debug.LogFormat ("[Game] RefreshGraveyard, isInstant {0} ", instant);
            var up = graveyard.up * placementAnimationData.GraveyardCardDistance;

            var animation = new AnimationQuery();

            Vector3 targetPosition = graveyard.position;
            Quaternion targetRotation = graveyard.rotation;

            int found = 0;

            for (int i = 0; i < gameSettings.GameCardPoolSize; i++) {
                var card = gameCardsData.GetRandom<Card>();
                if (card.CurrentLayout == null) {
                    var tPosition = targetPosition + up * found;

                    if (instant) {
                        card.SetPosition(tPosition);
                        card.SetRotation(targetRotation);
                    } else {
                        animation.AddToQuery(new MovementAction(card, tPosition, placementAnimationData.GraveyardPositionUpdateSpeed, placementAnimationData.GraveyardPositionCurve, placementAnimationData.GraveyardHeightCurve));
                        animation.AddToQuery(new RotateAction(card, targetRotation, placementAnimationData.GraveyardRotationUpdateSpeed, placementAnimationData.GraveyardRotationCurve));
                    }

                    found++;
                }
            }

            graveyardCounter = found;

            animation.Start(this, () => {
                onCompleted?.Invoke();

                for (int i = 0; i < gameSettings.GameCardPoolSize; i++) {
                    var card = gameCardsData.GetRandom<Card>();
                    if (card.CurrentLayout == null) {
                        // restore material.
                        card.SetMaterial(defaultCardMaterial);
                    }
                }
            });
        }

        /// <summary>
        /// Clears the game. All cards goes to graveyard.
        /// </summary>
        public void ClearGame (bool instant, Action onCompleted) {
            // reset game function query.
            gameFunctionQuery.Reset(this);

            // kill the game cards animations.
            for (int i = 0; i < gameSettings.GameCardPoolSize; i++) {
                var card = gameCardsData.GetRandom<Card>();
                if (card.CurrentAnimation != null) {
                    card.CurrentAnimation.Stop();
                    card.CurrentAnimation = null;
                }

                card.Placing(false);
                card.Placed(false);
                card.Interested(false, false);
                card.Selected(false);
            }

            if (showCardAnimation != null) {
                showCardAnimation.Stop();
                showCardAnimation = null;
            }

            if (colorAnimation != null) {
                colorAnimation.Stop();
                colorAnimation = null;
            }

            if (Players != null) {
                foreach (var player in Players) {
                    if (player != null) {
                        player.Clear();
                    }
                }
            }

            // clear layouts.
            for (int i = 0; i < 2; i++) {
                UserTables[i].Clear(true);
                OpponentTables[i].Clear(true);
                Decks[i].Clear(true);
            }

            RefreshGraveyard(instant, onCompleted);
        }

        public void NextTurn () {
            if (++currentTurn>=2) {
                currentTurn = 0;
            }

            UserTurn(currentTurn);
        }

        private void UserTurn(int user) {
            Debug.Log("[Game] User Turn => " + user);

            currentTurn = user;

            if (!isGameStarted) {
                return;
            }

            if (!Players[currentTurn].HaveAMove()) {
                GameOver(false);
                Debug.Log("[Game] No move left. Game is over.");
                return;
            }

            Decks[0].enabled = true; // enable local user deck interaction

            Players[user].OnTurn();

            // new turn event.
            EventUserTurn.OnTriggered?.Invoke(user);
        }

        public TableLayout GetTableLayoutByUser (int targetLayoutIndex) {
            TableLayout targetLayout;

            // find targetlayout by user.
            if (currentTurn == 1) { // opponent move.
                if (targetLayoutIndex < 2) {
                    targetLayout = OpponentTables[targetLayoutIndex];
                } else {
                    targetLayout = UserTables[targetLayoutIndex];
                }
            } else { // local user move.
                if (targetLayoutIndex < 2) {
                    targetLayout = UserTables[targetLayoutIndex];
                } else {
                    targetLayout = OpponentTables[targetLayoutIndex - 2];
                }
            }

            return targetLayout;
        }

        /// <summary>
        /// Show the given card with showOpponentCardPoint transform values.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="onCompleted"></param>
        public void ShowCard(Card card, Action onCompleted) {
            slideCardSoundClip.Play();

            // go to opponent show position first, to show the card to the user.
            showCardAnimation = new AnimationQuery();
            showCardAnimation.AddToQuery(new MovementAction(card, showOpponentCardPoint.position, placementAnimationData.ShowCardPositionSpeed, placementAnimationData.ShowCardPositionCurve));
            showCardAnimation.AddToQuery(new RotateAction(card, showOpponentCardPoint.rotation, placementAnimationData.ShowCardRotationSpeed, placementAnimationData.ShowCardRotationCurve));
            showCardAnimation.AddToQuery(new ScaleAction(card, Vector3.one, 10, AnimationCurve.Linear(0, 0, 1, 1)));

            showCardAnimation.Start(this, () => {
                // show tooltip.
                card.Interested (true);

                showCardAnimation = null;
                // wait for 2 seconds and hit.
                showCardAnimation = new AnimationQuery();
                showCardAnimation.AddToQuery(new TimerAction(2f));
                showCardAnimation.Start(this, ()=> { 
                    card.Interested(false);  
                    onCompleted?.Invoke(); 
                });
            });
        }

        /// <summary>
        /// End the game.
        /// </summary>
        /// <param name="isAborted">Is aborted by user?</param>
        public void GameOver (bool isAborted) {
            // clear dummy card.
            if (!isGameStarted) {
                return;
            }
            
            if (!isAborted && currentRound < gameSettings.RoundCount) {
                NextRound();
                return;
            }

            if (isAborted) {
                HardClear();
            }

            isGameStarted = false;

            // show the opponents hand.
            Decks[1].Open();

            EventGameOver.OnTriggered?.Invoke(isAborted);
        }
        
        private void NextRound () {
            if (isAIEnabled) {
                StartGameWithAI(true);
            } else {
                Gateway.OnNeedToSendCardData?.Invoke();
            }
        }

        private void HardClear() {
            void layoutClear (Layout[] layouts) {
                foreach (var layout in layouts) {
                    if (layout.currentQuery != null) {
                        layout.currentQuery.Stop();
                        layout.currentQuery = null;
                    }

                    layout.StopInteraction();
                }
            }
            
            layoutClear(Decks);
            layoutClear(UserTables);
            layoutClear(OpponentTables);

            ClearGame(false, null);
        }
    }
}

