using UnityEngine;
using CardGame.Animation;
using System;

namespace CardGame.Effects {
    public class MoverEffect : SkillEffect {
#pragma warning disable CS0649
        [Header("Movable skill effect properties.")]
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private AnimationCurve positionCurve;
        [SerializeField] private float animationSpeed = 1f;
#pragma warning restore CS0649

        public override void Play(Vector3 startPosition, Vector3 targetPosition, int range, Action<string> onCompleted) {
            base.Play(startPosition, targetPosition, range, onCompleted);

            PlayAnimation();

            currentAnimation = new AnimationQuery();
            currentAnimation.AddToQuery(new MovementAction(this, targetPosition, animationSpeed, positionCurve, heightCurve, true));

            currentAnimation.Start(this, () => {
                StopAnimation();
                currentAnimation = null;
                onCompleted?.Invoke(playThisParticleOnHit);
            });
        }
    }
}
