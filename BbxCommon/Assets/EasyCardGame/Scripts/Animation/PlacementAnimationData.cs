using UnityEngine;

namespace CardGame.Animation {
    [CreateAssetMenu(fileName = "PlacementAnimationData", menuName = "CardGame/Animation/PlacementAnimationData", order = 1)]
    public class PlacementAnimationData : ScriptableObject {
        [Header ("User plays the card. It will hit the ground by this animation parameters.")]
        public AnimationCurve PositionCurve;
        public AnimationCurve RotationCurve;
        public AnimationCurve HeightCurve;
        public float PositionUpdateSpeed = 1;
        public float RotationUpdateSpeed = 1;

        [Header ("Card will be showed to player before play by this animation parameters")]
        public AnimationCurve ShowCardPositionCurve;
        public AnimationCurve ShowCardRotationCurve;
        public float ShowCardPositionSpeed = 1;
        public float ShowCardRotationSpeed = 1;

        [Header("Graveyard")]
        public float GraveyardCardDistance = 0.05f;
        public AnimationCurve GraveyardPositionCurve;
        public AnimationCurve GraveyardRotationCurve;
        public AnimationCurve GraveyardHeightCurve;
        public float GraveyardPositionUpdateSpeed = 1;
        public float GraveyardRotationUpdateSpeed = 1;
        public Gradient GraveyardResurrectGradient;
        public float GraveyardResurrectUpdateSpeed = 1;
        public float GraveyardResurrectIntensity = 2;
    }
}

