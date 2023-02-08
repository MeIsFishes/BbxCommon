using UnityEngine;
using URandom = UnityEngine.Random;
using System;

namespace CardGame.Sounds {
    [CreateAssetMenu(fileName = "SoundClip", menuName = "CardGame/Sounds/SoundClip", order = 1)]
    public class SoundClip : ScriptableObject {
        private static AudioSource audioSource;
#pragma warning disable CS0649
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private float playOffset = 0.2f;
        [SerializeField] private float volume = 0.25f;
#pragma warning restore CS0649

        [NonSerialized] private float nextAvailable;

        public AudioClip GetClip () {
            return clips[URandom.Range (0, clips.Length)];
        }

        public void Play() {
            if (audioSource == null) {
                // create audio source.
                audioSource = new GameObject("audioSource").AddComponent<AudioSource>();
            }

            float time = Time.time;

            if (nextAvailable > time) {
                return;
            }

            nextAvailable = time + playOffset;

            audioSource.PlayOneShot(GetClip(), volume);
        }
    }
}
