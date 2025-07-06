using BbxCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BbxCommon
{
	public static class InputApi
	{
        #region Hotkeys

        #region Register
        public enum EHotkeyType
		{
            /// <summary>
            /// For the same hotkeys, only the last registered will be invoked.
            /// If there is a hotkey A with strict more keys than B, then B will be ignored.
            /// <para>For example, if there is a hotkey [Ctrl, Shift], and two hotkeys [Ctrl, Shift, A], then only the last registered [Ctrl, Shift, A] will be invoked.</para>
            /// </summary>
			Mutex,
            /// <summary>
            /// Hotkeys will be invoked independently.
            /// <para>For example, if there is a hotkey [Ctrl, Shift], and two hotkeys [Ctrl, Shift, A], they will all be invoked.</para>
            /// <para>However, each hotkey will be invoked only once until released.</para>
            /// </summary>
			Independent,
            /// <summary>
            /// Hotkeys will be invoked independently and be invoked every frame, until you release them.
            /// <para>For example, if there is a hotkey [Ctrl, Shift], and two hotkeys [Ctrl, Shift, A], they will all be invoked every frame.</para>
            /// </summary>
            IndependentAndEveryFrame,
		}

        /// <summary>
        /// Hotkeys may contains a series of key group.
        /// For example, a hotkey may be defined as [Ctrl, Shift, A].
		/// In this case, we store the hotkey as a tree structure one by one.
		/// Then there will be 3 HotkeyData. The root NextKey dictionary contains key [Ctrl], then contains a HotkeyData for [Shift], and then HotkeyData contains a HotkeyData for [A].
		/// When we reach [A], we can call the callback function.
        /// </summary>
        private class HotkeyData
        {
            public EHotkeyType HotkeyType;
            public List<Action> CallbackList = new();
			public Dictionary<int, HotkeyData> NextKey = new();
			public List<int> PreviousKeys = new();

            public void InvokeLast()
            {
                if (CallbackList.Count > 0)
                {
                    CallbackList[CallbackList.Count - 1]?.Invoke();
                }
            }

            public void InvokeAll()
            {
                SimplePool.Alloc(out List<Action> temp);
                temp.AddRange(CallbackList);
                for (int i = 0; i < temp.Count; i++)
                {
                    temp[i]?.Invoke();
                }
                temp.CollectToPool();
            }
        }

        private static Dictionary<EHotkeyType, HotkeyData> m_HotkeyDataDic = new();

        public static void RegisterHotkey(Action callback, EHotkeyType hotkeyType, List<int> keys)
        {
            if (keys == null || keys.Count == 0)
                return;
            var hotkeyData = GetOrCreateHotkeyData(GetHotkeyRoot(hotkeyType), keys);
            hotkeyData.HotkeyType = hotkeyType;
            hotkeyData.CallbackList.Add(callback);
        }

        public static void RegisterHotkey(Action callback, EHotkeyType hotkeyType, params int[] keys)
        {
            RegisterHotkey(callback, hotkeyType, keys.ToList());
        }

        public static void UnregisterHotkey(Action callback, EHotkeyType hotkeyType, List<int> keys)
        {
            if (keys == null || keys.Count == 0)
                return;
            var hotkeyData = GetOrCreateHotkeyData(GetHotkeyRoot(hotkeyType), keys);
            hotkeyData.CallbackList.Remove(callback);
        }

        public static void UnregisterHotkey(Action callback, EHotkeyType hotkeyType, params int[] keys)
        {
            UnregisterHotkey(callback, hotkeyType, keys.ToList());
        }

        private static HotkeyData GetHotkeyRoot(EHotkeyType hotkeyType)
        {
            if (m_HotkeyDataDic.TryGetValue(hotkeyType, out var root))
            {
                return root;
            }
            else
            {
                var newData = new HotkeyData();
                newData.HotkeyType = hotkeyType;
                m_HotkeyDataDic[hotkeyType] = newData;
                return newData;
            }
        }

        private static HotkeyData GetOrCreateHotkeyData(HotkeyData root, List<int> keys)
        {
            HotkeyData curData = root;
            for (int i = 0; i < keys.Count; i++)
            {
                if (curData.NextKey.TryGetValue(keys[i], out var nextData))
                {
                    curData = nextData;
                }
                else
                {
                    var newData = new HotkeyData();
                    curData.NextKey[keys[i]] = newData;
                    curData = newData;
                    for (int j = 0; j <= i; j++)
                    {
                        newData.PreviousKeys.Add(keys[j]);
                    }
                }
            }
            return curData;
        }
        #endregion

        #region Tick
        // InputApi tick should be called in project's main loop.
        // Code template:
        //
        // SimplePool.Alloc(out Dictionary<int, bool> keyStates);
        // InputApi.GetKeyStateRequestDic(keyStates);
		// foreach (var pair in keyStates)
		// {
		//     if (Input.IsKeyPressed((Key) pair.Key))
		//     {
		//         keyStates[pair.Key] = true;
        //     }
        // }
        // InputApi.Tick(keyStates);
        // keyStates.CollectToPool();
        //

        public class KeyState
        {
            public int KeyCode;
            public bool IsDown;
            public bool IsUp
            {
                get { return !IsDown; }
                set { IsDown = !value; }
            }
        }

        private static Dictionary<int, KeyState> m_KeyStateDic = new();
		private static List<HotkeyData> m_CurActiveHotkeys = new();

		public static void Tick(Dictionary<int, bool> keyStates)
		{
			foreach (var pair in keyStates)
			{
				if (m_KeyStateDic.TryGetValue(pair.Key, out var keyState))
				{
                    keyState.IsDown = pair.Value;
                }
			}
            // update hotkeys
			SimplePool.Alloc(out HashSet<HotkeyData> newActiveHotkeys); // cache active hotkeys for next frame
            for (int i = 0; i < m_CurActiveHotkeys.Count; i++)
			{
				var hotkeyData = m_CurActiveHotkeys[i];
                // check previous keys
                bool previousUp = false;
                for (int j = 0; j < hotkeyData.PreviousKeys.Count; j++)
                {
                    if (IsKeyUp(hotkeyData.PreviousKeys[j]))
                    {
                        previousUp = true;
                        break;
                    }
                    if (hotkeyData.HotkeyType == EHotkeyType.IndependentAndEveryFrame) // for IndependentAndEveryFrame, callback invokes every frame
                    {
                        hotkeyData.InvokeAll();
                    }
                }
                if (previousUp == false)
                {
                    newActiveHotkeys.TryAdd(hotkeyData);
                }
                // check next keys
                foreach (var pair in hotkeyData.NextKey)
                {
                    if (IsKeyDown(pair.Key))
                    {
                        if (hotkeyData.HotkeyType == EHotkeyType.Mutex)
                        {
                            newActiveHotkeys.TryAdd(pair.Value);
                            newActiveHotkeys.Remove(hotkeyData); // for Mutex, a new hotkey will replace the old one
                            if (pair.Value.CallbackList.Count > 0 &&
                                (pair.Value.NextKey == null || pair.Value.NextKey.Count == 0) &&
                                m_CurActiveHotkeys.Contains(pair.Value) == false) // callback should invokes
                            {
                                pair.Value.InvokeLast();
                                break;
                            }
                        }
                        else if (hotkeyData.HotkeyType == EHotkeyType.Independent || hotkeyData.HotkeyType == EHotkeyType.IndependentAndEveryFrame)
                        {
                            // for Independent, a single hotkey invokes once only
                            if (pair.Value.CallbackList.Count > 0 && m_CurActiveHotkeys.Contains(pair.Value) == false)
                            {
                                pair.Value.InvokeAll();
                            }
                            newActiveHotkeys.TryAdd(pair.Value); // add next key to active hotkeys
                        }
                    }
                }
            }
            // refresh active hotkeys
            m_CurActiveHotkeys.Clear();
            newActiveHotkeys.TryAdd(GetHotkeyRoot(EHotkeyType.IndependentAndEveryFrame)); // add root
            foreach (var pair in m_HotkeyDataDic)
            {
                bool hasHotkeyType = false;
                foreach (var hotkeyData in newActiveHotkeys)
                {
                    if (hotkeyData.HotkeyType == pair.Key)
                    {
                        hasHotkeyType = true;
                        break;
                    }
                }
                if (hasHotkeyType == false)
                {
                    newActiveHotkeys.TryAdd(pair.Value); // add root hotkey data if no active hotkeys of this type
                }
            }
            foreach (var hotkeyData in newActiveHotkeys)
            {
                m_CurActiveHotkeys.Add(hotkeyData);
            }
            newActiveHotkeys.CollectToPool();
        }

        public static void GetKeyStateRequestDic(Dictionary<int, bool> keyStates)
        {
            // add root hotkeys if no active hotkeys
            if (m_CurActiveHotkeys.Count == 0)
            {
                m_CurActiveHotkeys.AddRange(m_HotkeyDataDic.Values);
            }
            // clear m_KeyStateDic
            foreach (var pair in m_KeyStateDic)
            {
                SimplePool.Collect(pair.Value);
            }
            m_KeyStateDic.Clear();
            // add all keys in active hotkeys to keyStates
            for (int i = 0; i < m_CurActiveHotkeys.Count; i++)
            {
                var hotkeyData = m_CurActiveHotkeys[i];
                for (int j = 0; j < hotkeyData.PreviousKeys.Count; j++)
                {
                    keyStates.TryAdd(hotkeyData.PreviousKeys[j], false);
                    SimplePool.Alloc(out KeyState keyStateData);
                    keyStateData.KeyCode = hotkeyData.PreviousKeys[j];
                    keyStateData.IsDown = false;
                    m_KeyStateDic.TryAdd(hotkeyData.PreviousKeys[j], keyStateData);
                }
                foreach (var pair in hotkeyData.NextKey)
                {
                    keyStates.TryAdd(pair.Key, false);
                    SimplePool.Alloc(out KeyState keyStateData);
                    keyStateData.KeyCode = pair.Key;
                    keyStateData.IsDown = false;
                    m_KeyStateDic.TryAdd(pair.Key, keyStateData);
                }
            }
        }

		private static bool IsKeyDown(int keyCode)
        {
            if (m_KeyStateDic.TryGetValue(keyCode, out var keyState))
            {
                return keyState.IsDown;
            }
            return false;
        }

		private static bool IsKeyUp(int keyCode)
        {
            if (m_KeyStateDic.TryGetValue(keyCode, out var keyState))
            {
                return keyState.IsUp;
            }
            return true;
        }
        #endregion

        #endregion
    }
}
