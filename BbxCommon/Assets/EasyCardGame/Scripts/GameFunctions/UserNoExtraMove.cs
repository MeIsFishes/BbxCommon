using System;

namespace CardGame.GameFunctions {
    public class UserNoExtraMove : BaseFunction {
        public UserNoExtraMove(Game game) : base(game) {
            this.game = game;
        }

        public override void Trigger(Action onCompleted) {
            game.NextTurn();
            onCompleted?.Invoke();
        }
    }
}