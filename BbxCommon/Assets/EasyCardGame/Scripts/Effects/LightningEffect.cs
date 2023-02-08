using UnityEngine;
using CardGame.Animation;
using System;

namespace CardGame.Effects {
    public class LightningEffect : SkillEffect {
#pragma warning disable CS0649
        [SerializeField] private float rayWidth = 1;
#pragma warning restore CS0649

        public override void Play(Vector3 startPosition, Vector3 targetPosition, int range, Action<string> onCompleted) {
            base.Play(startPosition, targetPosition, range, onCompleted);

            float distance = Vector3.Distance(startPosition, targetPosition);
            Quaternion rot = Quaternion.LookRotation(targetPosition - startPosition);

            foreach (var particle in particleSystems) {
                particle.transform.rotation = rot;

                var shapeModule = particle.shape;
                shapeModule.scale = new Vector3(rayWidth, rayWidth, distance);
                shapeModule.position = new Vector3(0, 0, distance / 2);
            }

            PlayAnimation();

            onCompleted?.Invoke(playThisParticleOnHit);
        }
    }
}
