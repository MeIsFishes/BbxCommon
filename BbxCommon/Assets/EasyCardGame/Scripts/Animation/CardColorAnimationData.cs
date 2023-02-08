using UnityEngine;

namespace CardGame.Animation {
    [CreateAssetMenu(fileName = "CardColorAnimationData", menuName = "CardGame/Animation/CardColorAnimationData", order = 1)]
    public class CardColorAnimationData : ScriptableObject {
        [Tooltip ("Card got damaged in a table layout")]
        public Gradient damageGradient;

        [Tooltip ("Card got healed in a table layout.")]
        public Gradient healGradient;

        [Tooltip("Card is dead in a table layout.")]
        public Gradient dieGradient;

        [Tooltip("Skill card destroyed from users hand.")]
        public Gradient destroyGradient;

        [Tooltip("Card is interacted by pointer")]
        public Gradient interactionGradient;

        [Tooltip("Card is interaction false")]
        public Gradient interactionEndGradient;

        [Tooltip("Target mark color for offensive targets.")]
        public Color targetMarkColorOffensive;
        [Tooltip("Target mark color for defensive targets.")]
        public Color targetMarkColorDefensive;

        public Color hitEffectTextColorForDamage;
        public Color hitEffectTextColorForHealing;

        public float dieColorIntensity = 1;
        public float healColorIntensity = 1;
        public float damageColorIntensity = 1;
        public float destroyColorIntensity = 1;
        public float interactionColorIntensity = 1;

        public float targetMarkColorOffensiveIntensity = 1;
        public float targetMarkColorDefensiveIntensity = 1;

        public float damageEffectSpeed = 1;
        public float healEffectSpeed = 1;
        public float dieEffectSpeed = 1;
        public float destroyEffectSpeed = 1;
        public float interactionColorSpeed = 1;
    }
}

