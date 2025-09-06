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
        #endregion

        #region Profiler
        public class ProfilerData
        {
            public string Key;
            public long TimeMs => m_Stopwatch.ElapsedMilliseconds;
            public long TimeUs => m_Stopwatch.ElapsedUs();
            public long TimeNs => m_Stopwatch.ElapsedNs();
            private Dictionary<string, ProfilerData> m_DataDic = new();
            private Stopwatch m_Stopwatch = new();

            public ProfilerData GetData(string key)
            {
                m_DataDic.GetOrAdd(key, out var data);
                data.Key = key;
                return data;
            }

            public void BeginSample()
            {
                m_Stopwatch.Restart();
            }

            public void EndSample()
            {
                m_Stopwatch.Stop();
            }

            public void OutputTimeMs()
            {
                DebugApi.Log(Key + ": " + TimeMs + " ms");
            }

            public void OutputTimeUs()
            {
                DebugApi.Log(Key + ": " + TimeUs + " us");
            }

            public void OutputtTimeNs()
            {
                DebugApi.Log(Key + ": " + TimeNs + " ns");
            }

            public void EndSampleAndOutputTimeMs()
            {
                EndSample();
                OutputTimeMs();
            }

            public void EndSampleAndOutputTimeUs()
            {
                EndSample();
                OutputTimeUs();
            }

            public void EndSampleAndOutputTimeNs()
            {
                EndSample();
                OutputtTimeNs();
            }
        }

        private static ProfilerData m_ProfilerRoot = new();

        /// <summary>
        /// If you need to frequently use a sampler, consider saving the sampler to avoid repeated GetData calls.
        /// </summary>
        public static ProfilerData CreateSampler(string key)
        {
            return m_ProfilerRoot.GetData(key);
        }

        public static ProfilerData BeginSample(string key)
        {
            var data = m_ProfilerRoot.GetData(key);
            data.BeginSample();
            return data;
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

        public static long GetProfilerTimeNs(string key)
        {
            return m_ProfilerRoot.GetData(key).TimeNs;
        }
        #endregion
    }
}