using System;
using CardGame.GameEvents;
using CardGame.Networking;

namespace CardGame.GameFunctions {
    public class UserPass : BaseFunction {
        public UserPass (Game game) : base (game) {
            this.game = game;
        }

        public override void Trigger(Action onCompleted) {
            EventUserPass.OnTriggered?.Invoke(game.currentTurn);
            Gateway.OnGameNoExtraMove?.Invoke();
            onCompleted?.Invoke();
        }
    }
}
