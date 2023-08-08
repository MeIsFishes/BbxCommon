using UnityEngine;
using Sirenix.OdinInspector;

namespace Dcg
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Demos/Dcg/RoomData")]
    public class RoomData : ScriptableObject
    {
        public GameObject RoomPrefab;
        public Vector2 RoomSize;
        public Vector3 CharacterOffset;
        public Vector3 MonsterOffset;

        [FoldoutGroup("SpawnRoom"), Tooltip("Position offset once spawned.")]
        public Vector3 SpawnOffset;
        [FoldoutGroup("SpawnRoom"), Tooltip("Room will fix its offset after being spawned. " +
            "Evaluation 0 represents the offset position, and value 1 represents its original position.")]
        public AnimationCurve TransitionCurve;
    }
}
