using System;
using System.Collections.Generic;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#elif GODOT
using Godot;
#endif
using Stopwatch = System.Diagnostics.Stopwatch;

namespace BbxCommon
{
    public static class DebugApi
    {
        #region Log
        public static void Log(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.Log(args);
#elif GODOT
            GD.Print(args);
#endif
            Console.WriteLine("DebugApi.Log: ");
            Console.Write(args);
        }

        public static void LogWarning(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogWarning(args);
#elif GODOT
            GD.PushWarning(args);
#endif
            Console.WriteLine("DebugApi.LogWarning: ");
            Console.Write(args);
        }

        public static void LogError(object args)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogError(args);
#elif GODOT
            GD.PushError(args);
#endif
            Console.WriteLine("DebugApi.LogError: ");
            Console.Write(args);
        }

        public static void LogException(Exception e)
        {
#if UNITY_2017_1_OR_NEWER
            Debug.LogException(e);
#elif GODOT
            GD.PushError(e);
#endif

            Console.WriteLine("DebugApi.LogException: ");
            Console.Write(e);
        }
        #endregion

        #region Profiler
        private class ProfilerData
        {
            public string Key;
            public long TimeMs => m_Stopwatch.ElapsedMilliseconds;
            public long TimeUs => m_Stopwatch.ElapsedUs();
            private Dictionary<string, ProfilerData> m_DataDic = new();
            private Stopwatch m_Stopwatch = new();

            public ProfilerData GetData(string key)
            {
                return m_DataDic.GetOrAdd(key);
            }

            public void BeginSample()
            {
                m_Stopwatch.Restart();
            }

            public void EndSample()
            {
                m_Stopwatch.Stop();
            }
        }

        private static ProfilerData m_ProfilerRoot = new();

        public static void BeginSample(string key)
        {
            m_ProfilerRoot.GetData(key).BeginSample();
        }

        public static void EndSample(string key)
        {
            m_ProfilerRoot.GetData(key).EndSample();
        }

        public static long GetProfilerTimeMs(string key)
        {
            return m_ProfilerRoot.GetData(key).TimeMs;
        }

        public static long GetProfilerTimeUs(string key)
        {
            return m_ProfilerRoot.GetData(key).TimeUs;
        }
        #endregion
    }
}