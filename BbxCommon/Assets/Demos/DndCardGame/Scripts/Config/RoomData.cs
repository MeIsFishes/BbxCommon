using UnityEngine;
using BbxCommon;
using Sirenix.OdinInspector;

namespace Dcg
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Demos/Dcg/RoomData")]
    public class RoomData : BbxScriptableObject
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

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }
    }
}
