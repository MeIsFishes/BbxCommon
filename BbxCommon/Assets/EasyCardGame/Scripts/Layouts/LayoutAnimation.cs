using UnityEngine;
using CardGame.Sounds;

namespace CardGame.Layouts {
    [CreateAssetMenu(fileName = "LayoutAnimationData", menuName = "CardGame/Animation/LayoutAnimationData", order = 1)]
    public class LayoutAnimation : ScriptableObject {
        [System.Serializable]
        public class ProgressClip {
            [Range (0,1)]
            public float Time;
            public SoundClip Clip;
        }

        public ProgressClip[] ProgressClips;

        public float MovementSpeed = 1;
        public float RotateSpeed = 1;
        public float ScaleSpeed = 1;

        public AnimationCurve MovementCurve;

        [Tooltip ("Height addition over animation lifetime")]
        public AnimationCurve HeightCurve;

        public AnimationCurve RotateCurve;
        public AnimationCurve ScaleCurve;
    }
}

