
namespace CardGame.Animation
{
    public abstract class BaseAction {
        public float progress = 0;

        /// <summary>
        /// Update the action, returns true when completed.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="animationSettings"></param>
        /// <returns></returns>
        public virtual bool Update (in float deltaTime) { return false; }

        /// <summary>
        /// Finished the animation directly.
        /// </summary>
        public virtual void Finish () {
        }
    }
}