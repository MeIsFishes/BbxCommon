using UnityEngine;

namespace BbxCommon
{
    public static class DebugApi
    {
        public static void Log(object args)
        {
            Debug.Log(args);
        }

        public static void LogWarning(object args)
        {
            Debug.LogWarning(args);
        }

        public static void LogError(object args)
        {
            Debug.LogError(args);
        }
    }
}
