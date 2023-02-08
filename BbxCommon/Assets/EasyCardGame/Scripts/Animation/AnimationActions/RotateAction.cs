using UnityEngine;

namespace CardGame.Animation {
    public class RotateAction : BaseAction {
        private readonly Quaternion startRotation;
        private readonly Quaternion targetRotation;
        private readonly float speed;
        private readonly AnimationCurve curve;
        private readonly IMovable rotatable;

        public RotateAction(IMovable rotatable, Quaternion targetRotation, float speed, AnimationCurve curve) {
            startRotation = rotatable.GetRotation();

            this.targetRotation = targetRotation;
            this.speed = speed;
            this.curve = curve;
            this.rotatable = rotatable;
        }

        public sealed override bool Update(in float deltaTime) {
            progress = Mathf.Min(progress + deltaTime * speed, 1);

            var newRotation = Quaternion.Slerp (startRotation, targetRotation, curve.Evaluate (progress));

            rotatable.SetRotation(newRotation);

            return progress == 1;
        }

        public override void Finish() {
            base.Finish();

            rotatable.SetRotation(targetRotation);
        }
    }
}
