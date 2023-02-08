using CardGame.Animation;
using CardGame.Sounds;

using System;
using UnityEngine;

namespace CardGame.Effects {
    public abstract class SkillEffect : MonoBehaviour, IMovable {
#pragma warning disable CS0649
        [Tooltip ("When this effects find the target, this particle will be played. If you make it null or empty, means no effect on hit.")]
        [SerializeField] protected string playThisParticleOnHit;

        [Header ("Objects will be animated.")]
        [SerializeField] protected ParticleSystem[] particleSystems;
        [SerializeField] protected SoundClip[] soundClips;
#pragma warning restore CS0649

#if UNITY_EDITOR
        public string HitParticle => playThisParticleOnHit;
#endif

        /// <summary>
        /// Current animation query.
        /// </summary>
        public AnimationQuery currentAnimation;

        /// <summary>
        /// Particles and audios should have playOnAwake = false. We will play them manually via this script.
        /// </summary>
        private void OnValidate() {
            if (particleSystems != null) {
                foreach (var particle in particleSystems) {
                    if (particle == null)
                        continue;

                    var main = particle.main;
                    main.playOnAwake = false;
                }
            }
        }

        public virtual void Stop() {
            if (currentAnimation != null) {
                currentAnimation.Stop();
                currentAnimation = null;
            }
        }

        protected void PlayAnimation () {
            foreach (var particle in particleSystems) {
                if (particle == null)
                    continue;
                particle.Play(true);
            }

            foreach (var sound in soundClips) {
                if (sound == null)
                    continue;

                sound.Play();
            }
        }

        protected void StopAnimation () {
            foreach (var particle in particleSystems) {
                if (particle == null)
                    continue;
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        protected void Clear (Vector3 position) {
            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }

            if (currentAnimation != null) {
                currentAnimation.Stop();
            }

            SetPosition(position);
        }

        public virtual void Play (Vector3 startPosition, Vector3 targetPosition, int range, Action<string> onCompleted) {
            Clear(startPosition);
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Quaternion GetRotation() {
            return transform.rotation;
        }


        public void SetPosition(Vector3 targetPosition) {
            transform.position = targetPosition;
        }

        public void SetRotation(Quaternion targetRotation) {
            transform.rotation = targetRotation;
        }
    }
}

