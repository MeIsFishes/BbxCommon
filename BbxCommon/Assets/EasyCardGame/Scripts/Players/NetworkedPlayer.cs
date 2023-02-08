using UnityEngine;

namespace CardGame.Players {
    public class NetworkedPlayer : Player {
        public NetworkedPlayer (Game game, int layoutIndex) : base(game, layoutIndex) {

        }

        public override void AimFromCard(int cardLayoutIndex, int cardIndex) {
            base.AimFromCard(cardLayoutIndex, cardIndex);

            Debug.Log("[NetworkedPlayer] AimFromCard.");
        }

        public override void OnTurn() {
            base.OnTurn();

            Debug.Log("[NetworkedPlayer] OnTurn.");
        }
    }
}
