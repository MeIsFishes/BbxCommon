using UnityEngine;

#if PUN_2_19_OR_NEWER
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
#endif

namespace CardGame.Networking.Photon {
    public class PhotonManager :
#if PUN_2_19_OR_NEWER 
        MonoBehaviourPunCallbacks, IOnEventCallback
#else
        MonoBehaviour
#endif
        {

        #region event codes.
        public const byte UserCardEventCode = 1;
        public const byte UserPlacedCardEventCode = 2;
        public const byte UserTargetedCardEventCode = 3;
        public const byte WhoStartsEventCode = 4;
        public const byte UserPassEventCode = 5;
        public const byte UserNoExtraMoveEventCode = 6;
        public const byte UserRequestedForCardData = 7;
        #endregion

#pragma warning disable CS0649
        private PhotonBridge bridge;
#pragma warning restore CS0649

        private void Start () {
            // create photon brigde to the gateway.
            Gateway.Instance = bridge = new PhotonBridge();
#if PUN_2_19_OR_NEWER
            var view = gameObject.AddComponent<PhotonView>();
            view.ViewID = 0;
#else
            Debug.LogError("[PhotonGateway] PhotonGateway won't work because Photon is not found in the project. Please check EnablingPhotonMultiplayer documentation.");
#endif
        }

#if PUN_2_19_OR_NEWER
        public override void OnConnectedToMaster() {
            Debug.Log("OnConnectedToMaster() was called by PUN.");
            Gateway.OnConnectedToServer?.Invoke();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            base.OnPlayerEnteredRoom(newPlayer);

            Debug.LogFormat("OnPlayerEntered {0}", newPlayer.NickName);

            if (!newPlayer.IsLocal) {
                Gateway.OnPlayerConnected?.Invoke(newPlayer.NickName);
            }
        }

        public override void OnLeftRoom() {
            base.OnLeftRoom();
            Debug.Log("We left the room.");
            Gateway.OnLeftGame ?.Invoke();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            base.OnPlayerLeftRoom(otherPlayer);

            Debug.LogFormat("OnPlayerLeft {0}", otherPlayer.NickName);
            Gateway.OnPlayerDisconnected?.Invoke(otherPlayer.NickName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
            Gateway.OnJoinGameFailed?.Invoke();
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            bridge.OnJoinRandomFailed();
        }

        public override void OnJoinedRoom() {
            Debug.Log("OnJoinedRoom ()");
            Gateway.OnJoinedGame?.Invoke();
        }

        public void OnEvent(EventData photonEvent) {
            byte eventCode = photonEvent.Code;

            object[] data = (object[])photonEvent.CustomData;

            Debug.LogFormat ("[PhotonManager] OnGameEvent => {0}", eventCode);

            switch (eventCode) {
                case UserCardEventCode:
                    var cardData = (string[])data[0];
                    var deckId = (string)data[1];
                    Gateway.OnCardData?.Invoke(PhotonNetwork.LocalPlayer.ActorNumber == photonEvent.Sender, cardData, deckId);
                    break;

                case UserPlacedCardEventCode:
                    Gateway.OnGamePlacementData((int)data[0], (int)data[1], (int)data[2]);
                    break;

                case UserTargetedCardEventCode:
                    Gateway.OnGameTargetData((int)data[0], (int)data[1], (int)data[2], (int)data[3]);
                    break;

                case WhoStartsEventCode:
                    Gateway.OnWhoStart?.Invoke((bool)data[0]);
                    break;

                case UserPassEventCode:
                    Gateway.OnGamePass?.Invoke();
                    break;

                case UserNoExtraMoveEventCode:
                    Gateway.OnGameNoExtraMove?.Invoke();
                    break;

                case UserRequestedForCardData:
                    if (PhotonNetwork.LocalPlayer.ActorNumber != photonEvent.Sender) {
                        Gateway.OnCardDataRequestReceived?.Invoke((bool)data[0]);
                    }
                    break;
            }
        }
#endif
    }
}
