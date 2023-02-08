using CardGame.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame.UI {
    public class DefaultUINavigationMember : NavigationMember, IScalable {
        private AnimationQuery currentAnimation;

#pragma warning disable CS0649
        [SerializeField] private Vector3 defaultScale;
        [SerializeField] private Vector3 selectedScale;
        [SerializeField] private float scaleAnimationSpeed;
        [SerializeField] private AnimationCurve scaleCurve;
        [SerializeField] private UIAnimatedPanel selectedImage;
#pragma warning restore CS0649

        public Vector3 GetScale() {
            return transform.localScale;
        }

        public void SetScale(Vector3 scale) {
            transform.localScale = scale;
        }

        public override void Select() {
            base.Select();

            if (selectedImage != null) {
                selectedImage.Open();
            }

            if (currentAnimation != null) {
                currentAnimation.Stop();
            }

            if (gameObject.activeInHierarchy) {
                currentAnimation = new AnimationQuery();
                currentAnimation.AddToQuery(new ScaleAction(this, selectedScale, scaleAnimationSpeed, scaleCurve));
                currentAnimation.Start(this, () => {
                    currentAnimation = null;
                });
            }
        }

        public override void DeSelect() {
            base.DeSelect();

            if (selectedImage != null) {
                selectedImage.Close();
            }

            if (currentAnimation != null) {
                currentAnimation.Stop();
            }

            if (gameObject.activeInHierarchy) {
                currentAnimation = new AnimationQuery();
                currentAnimation.AddToQuery(new ScaleAction(this, defaultScale, scaleAnimationSpeed, scaleCurve));
                currentAnimation.Start(this, null);
            }
        }
    }
}
