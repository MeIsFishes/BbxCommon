using System;
using URandom = UnityEngine.Random;
using UnityEngine;
using CardGame.GameEvents;

namespace CardGame.Networking {
    public abstract class Gateway {

        /// <summary>
        /// This is literally the incoming user data. Hero name, cards etc.
        /// </summary>
        public delegate void CardDataDelegate(bool isLocalUser, string[] cards, string deckId);

        /// <summary>
        /// Network gateway instance.
        /// </summary>
        public static Gateway Instance;

        /// <summary>
        /// Currently connected to server?
        /// </summary>
        public virtual bool IsConnected => false;

        /// <summary>
        /// It's clear. Are we master of the lobby?
        /// </summary>
        public virtual bool AreWeLobbyMaster => false;

        // room events.
        public static Action <string> OnPlayerConnected;
        public static Action <string> OnPlayerDisconnected;
        public static Action <string[], string, string[], string, bool> OnStartGame;
        public static Action <bool> OnAbortGame;
        //

        // game events
        public static Action<bool> OnWhoStart;

        /// <summary>
        /// This is literally the incoming user data. Hero name, cards etc.
        /// </summary>
        public static CardDataDelegate OnCardData;

        public static Action<int, int, int> OnGamePlacementData;
        public static Action<int, int, int, int> OnGameTargetData;
        public static Action OnGamePass;
        public static Action OnGameNoExtraMove;

        // localuser events.
        public static Action OnConnectedToServer;
        public static Action OnDisconnectedFromServer;
        public static Action<string> OnConnectionFailed;
        public static Action OnJoinedGame;
        public static Action OnJoinGameFailed;
        public static Action OnLeftGame;
        public static Action OnNeedToSendCardData;
        public static Action<bool> OnCardDataRequestReceived;
        public static Action OnOpponentWantsReMatch;
        //

        // requests.
        public static Action OnConnectToServerRequest;
        public static Action OnQuickMatchRequest;
        public static Action OnPlayWithAIRequest;
        //

        private string[] UserDeck;
        private string[] OpponentsDeck;
        private string UserDeckId;
        private string OpponentsDeckId;

        private bool opponentWantsReMatch;

        public Gateway() {
            OnCardData += CardDataReceived;
            OnCardDataRequestReceived += CardDataRequestReceived;

            /// We have been dropped from the server.
            OnLeftGame += () => {
                cleanGame();
            };

            /// We left the game.
            OnLeftGame += () => {
                cleanGame();
            };

            /// Someone disconnected. The game should stop.
            OnPlayerDisconnected += (nickName) => {
                cleanGame();
            };

            // Some other user connected. Force for start game.
            OnPlayerConnected += (nickName) => {
                StartMatch(true);
            };

            EventGameOver.OnTriggered += (isAborted) => {
                cleanGame();
            };

            void cleanGame () {
                UserDeck = null;
                OpponentsDeck = null;
            }
        }

        private void CardDataReceived(bool isLocalUser, string[] data, string deckId) {
            Debug.LogFormat("[Gateway] Incoming card data, is from local user? => {0}", isLocalUser);
            // TODO use deck id
            if (isLocalUser) {
                UserDeck = data;
                UserDeckId = deckId;
            } else {
                OpponentsDeck = data;
                OpponentsDeckId = deckId;
            }

            // check if both received.
            if (UserDeck != null && OpponentsDeck != null) {
                OnStartGame?.Invoke(UserDeck, UserDeckId, OpponentsDeck, OpponentsDeckId, !IsConnected);

                // clear received decks.
                OpponentsDeck = null;
                UserDeck = null;
                OpponentsDeck = null;
                OpponentsDeckId = null;

                // decide who starts?
                SendWhoStarts();
            } else {
                Debug.Log("[Gateway] Still waiting for opponents card data.");
            }
        }

        public virtual void Connect() {
            OnConnectToServerRequest?.Invoke();
        }

        protected virtual void Disconnect () {
            if (IsConnected) {
                OnDisconnectedFromServer?.Invoke();
            }
        }

        public virtual void LeaveGame () {
            Disconnect();
            OnLeftGame?.Invoke();
        }

        public virtual void QuickMatch () {
            OnQuickMatchRequest?.Invoke();
        }

        public virtual void PlayWithAI () {
            if (IsConnected) {
                Disconnect(); // disconnect first.
            }

            OnPlayWithAIRequest?.Invoke();
        }

        /// <summary>
        /// Want to create a match with the opponent.
        /// </summary>
        /// <param name="force">forced = false, means request only. (For rematches)</param>
        public virtual void StartMatch (bool isForced) {
            if (isForced || opponentWantsReMatch) {
                /// send our card data directly.
                OnNeedToSendCardData?.Invoke(); // opponent & us will get OnCardData.
            }

            RequestCardData(isForced || opponentWantsReMatch);
            opponentWantsReMatch = false;
        }

        protected virtual void RequestCardData (bool isForced) {

        }

        private void CardDataRequestReceived (bool isForced) {
            if (!isForced) {
                opponentWantsReMatch = true;
                OnOpponentWantsReMatch?.Invoke();
            } else {
                OnNeedToSendCardData?.Invoke();
            }
        }

        #region game messages
        /// <summary>
        /// will the room master start the game?
        /// Default virtual method is for playing with AI.
        /// Online overridings should pass the calling this base method.
        /// </summary>
        /// <param name="willMasterStart"></param>
        public virtual void SendWhoStarts () {
            OnWhoStart?.Invoke(URandom.Range(0, 2) == 1);
        }

        /// <summary>
        /// Send card data for online games.
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="heroName"></param>
        public virtual void SendCardDataToNetwork (string[] cardData, string deckId) {
            
        }

        /// <summary>
        /// Send card data for offline games.
        /// </summary>
        /// <param name="cardData"></param>
        /// <param name="heroName"></param>
        /// <param name="isLocalUser"></param>
        public virtual void SendCardDataLocalGame (string[] cardData, string deckId, bool isLocalUser) {
            OnCardData?.Invoke(isLocalUser, cardData, deckId);
        }
        
        /// <summary>
        /// A user sends placement data.
        /// </summary>
        /// <param name="layoutMemberIndex"></param>
        /// <param name="targetLayoutIndex"></param>
        /// <param name="targetMemberIndex"></param>
        public virtual void SendPlacementData (int layoutMemberIndex, int targetLayoutIndex, int targetMemberIndex) {
            OnGamePlacementData?.Invoke(layoutMemberIndex, targetLayoutIndex, targetMemberIndex);
        }

        /// <summary>
        /// A user sends targeting data.
        /// </summary>
        /// <param name="cardLayoutIndex"></param>
        /// <param name="cardIndex"></param>
        /// <param name="targetLayoutIndex"></param>
        /// <param name="targetCardIndex"></param>
        public virtual void SendTargetData (int cardLayoutIndex, int cardIndex, int targetLayoutIndex, int targetCardIndex) {
            OnGameTargetData?.Invoke(cardLayoutIndex, cardIndex, targetLayoutIndex, targetCardIndex);
        }

        /// <summary>
        /// User passed.
        /// </summary>
        public virtual void SendPass () {
            OnGamePass?.Invoke();
        }

        /// <summary>
        /// User says no extra move I will play.
        /// </summary>
        public virtual void SendNoExtraMove () {
            OnGameNoExtraMove?.Invoke();
        }
        #endregion
    }
}

