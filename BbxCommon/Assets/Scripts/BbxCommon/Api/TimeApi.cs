using UnityEngine;

namespace BbxCommon
{
    public static class TimeApi
    {
        public static float Time => UnityEngine.Time.time;
        public static float DeltaTime => UnityEngine.Time.deltaTime;
    }
}
