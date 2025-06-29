using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public enum EEvent
	{
		// common
		EditorDataStoreRefresh,
		CurSelectTaskNodeChanged,
		// timeline
		TimelineTasksChanged,
		TimelineMaxTimeChanged,
		TimelineNodeStartTimeOrDurationChanged,
	}

	public static class EventBus
	{
		private static Dictionary<EEvent, Action> m_EventCallbackDic = new();

		public static void RegisterEvent(EEvent e, Action action)
		{
			if (m_EventCallbackDic.ContainsKey(e))
				m_EventCallbackDic[e] += action;
			else
				m_EventCallbackDic[e] = action;
		}

		public static void UnregisterEvent(EEvent e, Action action)
		{
			if (m_EventCallbackDic.ContainsKey(e))
				m_EventCallbackDic[e] -= action;
		}

		public static void DispatchEvent(EEvent e)
		{
			if (m_EventCallbackDic.ContainsKey(e))
				m_EventCallbackDic[e]();
		}
	}
}
