using UnityEngine;
using Sirenix.OdinInspector;

namespace Nnp
{
    [CreateAssetMenu(fileName = "PlayerSettingAsset", menuName = "Demos/Nnp/PlayerSettingAsset")]
    public class PlayerSettingAsset : ScriptableObject
    {
        [FoldoutGroup("Attributes")]
        public int MaxHp;
        [FoldoutGroup("Attributes")]
        public int Attack;
        [FoldoutGroup("Attributes")]
        public float WalkSpeed;
        [FoldoutGroup("Attributes")]
        public float RunSpeed;

        [FoldoutGroup("Animations")]
        public string IdleAnimation;
        [FoldoutGroup("Animations")]
        public string WalkAnimation;
        [FoldoutGroup("Animations")]
        public string RunAnimation;
        [FoldoutGroup("Animations")]
        public string BeHitAnimation;
    }
}
