using System;
using UnityEngine;
using CardGame.Networking;
using UnityEngine.Events;

namespace CardGame.GameEvents {
    public class GameInputs : MonoBehaviour {
        [Serializable] public class UnityStringEvent : UnityEvent <string> {}
        [Serializable] public class UnityIntEvent : UnityEvent<int> { }

#pragma warning disable CS0649
        [Tooltip ("True means, even when this object gets disabled. Event listening. will stay.")]
        [SerializeField] private bool isStaticListener = false;
        [SerializeField] private UnityEvent onConnectToServerRequest;
        [SerializeField] private UnityStringEvent onConnectionFailed;
        [SerializeField] private UnityEvent onConnectedToServer;
        [SerializeField] private UnityEvent onJoinedGame;
        [SerializeField] private UnityEvent onJoinFailed;
        [SerializeField] private UnityStringEvent onPlayerConnected;
        [SerializeField] private UnityStringEvent onPlayerDisconnected;
        [SerializeField] private UnityEvent onLeftGame;
        [SerializeField] private UnityEvent onDisconnectedFromServer;
        [SerializeField] private UnityEvent onQuickMatchRequest;
        [SerializeField] private UnityEvent onRematchRequestFromOpponent;
#pragma warning restore CS0649

        private bool listened = false;

        private void OnEnable() {
            if (listened)
                return;

            Gateway.OnConnectToServerRequest += ConnectToServerRequest;
            Gateway.OnConnectionFailed += ConnectionFailed;
            Gateway.OnPlayerConnected += PlayerConnected;
            Gateway.OnPlayerDisconnected += PlayerDisconnected;
            Gateway.OnQuickMatchRequest += QuickMatchRequest;
            Gateway.OnJoinedGame += JoinedLobby;
            Gateway.OnJoinGameFailed += JoinFailed;
            Gateway.OnLeftGame += LeftGame;
            Gateway.OnConnectedToServer += ConnectedToServer;
            Gateway.OnDisconnectedFromServer += DisconnectedFromServer;
            Gateway.OnCardDataRequestReceived += CardDataRequestReceived;
        }

        private void OnDisable() {
            if (isStaticListener)
                return;

            Gateway.OnConnectToServerRequest -= ConnectToServerRequest;
            Gateway.OnConnectionFailed -= ConnectionFailed;
            Gateway.OnPlayerConnected -= PlayerConnected;
            Gateway.OnPlayerDisconnected -= PlayerDisconnected;
            Gateway.OnQuickMatchRequest -= QuickMatchRequest;
            Gateway.OnJoinedGame -= JoinedLobby;
            Gateway.OnJoinGameFailed -= JoinFailed;
            Gateway.OnLeftGame -= LeftGame;
            Gateway.OnConnectedToServer -= ConnectedToServer;
            Gateway.OnDisconnectedFromServer -= DisconnectedFromServer;
            Gateway.OnCardDataRequestReceived -= CardDataRequestReceived;
        }

        private void OnDestroy() {
            isStaticListener = false;
            OnDisable();
        }

        private void ConnectToServerRequest() { onConnectToServerRequest?.Invoke(); }
        private void ConnectionFailed(string reason) { onConnectionFailed?.Invoke(reason); }
        private void ConnectedToServer() { onConnectedToServer?.Invoke(); }
        private void JoinedLobby() { onJoinedGame?.Invoke(); }
        private void JoinFailed() { onJoinFailed?.Invoke(); }
        private void PlayerConnected(string nick) { onPlayerConnected?.Invoke(nick); }
        private void PlayerDisconnected(string nick) { onPlayerDisconnected?.Invoke(nick); }
        private void LeftGame() { onLeftGame?.Invoke(); }
        private void DisconnectedFromServer() { onDisconnectedFromServer?.Invoke(); }
        private void QuickMatchRequest() { onQuickMatchRequest?.Invoke(); }

        private void CardDataRequestReceived(bool isForced) { 
            if (!isForced) {
                onRematchRequestFromOpponent?.Invoke();
            }
        }
       
        public void GoOnline () {
            Gateway.Instance.Connect();
        }

        public void QuickMatch () {
            Gateway.Instance.QuickMatch();
        }

        public void PlayWithAI () {
            Gateway.Instance.PlayWithAI();
        }

        public void LeaveGame () {
            Debug.Log("[LeaveGame] Leave");
            Gateway.Instance.LeaveGame();
        }

        public void ReMatchRequest () {
            Debug.Log("[GameInputs] ReMatch request");
            Gateway.Instance.StartMatch(false);
        }
    }
}
