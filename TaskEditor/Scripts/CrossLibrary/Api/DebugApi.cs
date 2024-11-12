#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif
#if GODOT
using Godot;
#endif

namespace BbxCommon
{
    public static class DebugApi
    {
        public static void Log(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log(args);
#endif
#if GODOT
            GD.Print(args);
#endif
        }

        public static void LogWarning(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(args);
#endif
#if GODOT
            GD.PushWarning(args);
#endif
        }

        public static void LogError(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogError(args);
#endif
#if GODOT
            GD.PushError(args);
#endif  
        }
    }
}