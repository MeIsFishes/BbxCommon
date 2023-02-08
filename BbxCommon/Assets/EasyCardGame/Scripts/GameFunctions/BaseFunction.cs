using System;
using CardGame.Animation;

namespace CardGame.GameFunctions {
    public abstract class BaseFunction {
        protected Game game;
        public BaseFunction (Game game) {
            this.game = game;
        }

        protected AnimationQuery currentAnimation;

        public virtual void Clear () {
            if (currentAnimation != null) {
                currentAnimation.Stop();
                currentAnimation = null;
            }
        }

        /// <summary>
        /// Triggers the game action.
        /// </summary>
        public virtual void Trigger (Action onCompleted) {

        }
    }
}
