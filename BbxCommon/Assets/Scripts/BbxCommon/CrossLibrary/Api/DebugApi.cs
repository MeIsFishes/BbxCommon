using System;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#elif GODOT
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
#elif GODOT
            GD.Print(args);
#endif
        }

        public static void LogWarning(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(args);
#elif GODOT
            GD.PushWarning(args);
#endif
        }

        public static void LogError(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogError(args);
#elif GODOT
            GD.PushError(args);
#endif  
        }

        public static void LogException(Exception e)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogException(e);
#elif GODOT
            GD.PushError(e);
#endif
        }
    }
}