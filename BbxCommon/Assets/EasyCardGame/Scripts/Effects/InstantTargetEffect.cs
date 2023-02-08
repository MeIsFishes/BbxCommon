using UnityEngine;
using CardGame.Animation;
using System;

namespace CardGame.Effects {
    public class InstantTargetEffect : SkillEffect {
#pragma warning disable CS0649
        [SerializeField] private float waitForSecondsBeforePlay;
#pragma warning restore CS0649

        public override void Play(Vector3 startPosition, Vector3 targetPosition, int range, Action<string> onCompleted) {
            Clear(targetPosition);
            currentAnimation = new AnimationQuery();
            currentAnimation.AddToQuery(new TimerAction(waitForSecondsBeforePlay));
            currentAnimation.Start(this, () => {
                PlayAnimation();
                onCompleted?.Invoke(playThisParticleOnHit);
            });
        }
    }
}
