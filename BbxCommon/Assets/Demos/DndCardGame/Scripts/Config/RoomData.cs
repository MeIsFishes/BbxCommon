using UnityEngine;

namespace Dcg
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Demos/Dcg/RoomData")]
    public class RoomData : ScriptableObject
    {
        public GameObject RoomPrefab;
        public Vector2 RoomSize;
        public Vector3 CharacterOffset;
        public Vector3 MonsterOffset;
    }
}
