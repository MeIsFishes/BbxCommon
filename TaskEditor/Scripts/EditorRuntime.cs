using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	/// <summary>
	/// Data that exists only when editing tasks.
	/// </summary>
	public static class EditorRuntime
	{
		private static string m_BindingContextType;
		private static TaskContextExportInfo m_BindingContextInfo;
		public static string BindingContextType
		{
			get => m_BindingContextType;
			set
			{
				if (m_BindingContextType != value)
				{
					var info = EditorDataStore.GetTaskContextInfo(value);
					if (info != null)
					{
						m_BindingContextType = value;
						m_BindingContextInfo = info;
						OnCurSelectTaskNodeChanged?.Invoke();
					}
				}
			}
		}
		public static TaskContextExportInfo BindingContextInfo => m_BindingContextInfo;
		public static Action OnBindingContextTypeChanged;

		private static TaskNode m_CurSelectTaskNode;
		public static TaskNode CurSelectTaskNode
		{
			get => m_CurSelectTaskNode;
			set
			{
				if (value != m_CurSelectTaskNode)
				{
					m_CurSelectTaskNode = value;
                    OnCurSelectTaskNodeChanged?.Invoke();
                }
			}
		}
		public static Action OnCurSelectTaskNodeChanged;
	}
}
