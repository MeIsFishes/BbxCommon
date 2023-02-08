#if PUN_2_19_OR_NEWER
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#endif
using UnityEngine;

namespace CardGame.Networking.Photon {
    public class PhotonBridge : Gateway {
        public override void Connect() {
#if PUN_2_19_OR_NEWER
            base.Connect();

            PhotonNetwork.ConnectUsingSettings();
#endif
        }

        protected override void Disconnect() {
#if PUN_2_19_OR_NEWER
            base.Disconnect();
            PhotonNetwork.Disconnect();
#endif
        }

#if PUN_2_19_OR_NEWER
        public override bool IsConnected => PhotonNetwork.IsConnected;
        public override bool AreWeLobbyMaster => PhotonNetwork.IsMasterClient;

        public override void QuickMatch() {
            base.QuickMatch();

            Debug.Log("[PhotonBridge] QuickMatch ()");

            var join = PhotonNetwork.JoinRandomRoom();
            Debug.Log("join => " + join);

            if (!join) {
                Debug.LogError("QuickMatch failed with a fatal issue.");
                OnJoinGameFailed?.Invoke();
            }
        }

        public void OnJoinRandomFailed() {
            CreateOrJoinGame();
        }

        private void CreateOrJoinGame() {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.JoinOrCreateRoom(System.Guid.NewGuid ().ToString (), roomOptions, null);
            Debug.Log("[PhotonBridge] Creating game..");
        }

        public override void SendWhoStarts() {
            if (!IsConnected) {
                base.SendWhoStarts();
            } else {
                object[] content = new object[] { Random.Range (0, 2) > 1 };
                SendPhotonEvent(PhotonManager.WhoStartsEventCode, content);
            }
        }

        protected override void RequestCardData(bool isForced) {
            object[] content = new object[1] { isForced };
            SendPhotonEvent(PhotonManager.UserRequestedForCardData, content);
        }

        public override void SendCardDataToNetwork(string[] cardData, string deckId) {
            base.SendCardDataToNetwork(cardData, deckId);

            if (IsConnected) {
                object[] content = new object[] { cardData, deckId };
                SendPhotonEvent(PhotonManager.UserCardEventCode, content);
            } else {
                Debug.LogError("[PhotonBridge] This is a network message from gateway, but there is no connection.");
            }
        }

        public override void SendPlacementData(int layoutMemberIndex, int targetLayoutIndex, int targetMemberIndex) {
            if (!IsConnected) {

                base.SendPlacementData(layoutMemberIndex, targetLayoutIndex, targetMemberIndex);

            } else {
                object[] content = new object[] { layoutMemberIndex, targetLayoutIndex, targetMemberIndex };
                SendPhotonEvent(PhotonManager.UserPlacedCardEventCode, content);
            }
        }

        public override void SendTargetData(int cardLayoutIndex, int cardIndex, int targetLayoutIndex, int targetCardIndex) {
            if (!IsConnected) {
                base.SendTargetData(cardLayoutIndex, cardIndex, targetLayoutIndex, targetCardIndex);

            } else {
                object[] content = new object[] { cardLayoutIndex, cardIndex, targetLayoutIndex, targetCardIndex };
                SendPhotonEvent(PhotonManager.UserTargetedCardEventCode, content);
            }
        }

        public override void SendPass() {
            if (!IsConnected) {
                base.SendNoExtraMove();
            } else {
                object[] content = new object[0];
                SendPhotonEvent(PhotonManager.UserPassEventCode, content);
            }
        }

        public override void SendNoExtraMove() {
            if (!IsConnected) {
                base.SendNoExtraMove();
            } else {
                object[] content = new object[0];
                SendPhotonEvent(PhotonManager.UserNoExtraMoveEventCode, content);
            }
        }

        private void SendPhotonEvent (byte code, object[] content) {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(code, content, raiseEventOptions, SendOptions.SendReliable);
        }
#endif
    }
}

