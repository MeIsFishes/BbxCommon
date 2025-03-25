using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    #region Base Struct
    public partial class TaskEditField : GodotObject
    {
        public string FieldName;
        public TaskExportTypeInfo TypeInfo;
        public ETaskFieldValueSource ValueSource;
        public string Value;
    }

	public partial class TaskEditData : GodotObject
	{
		public string TaskType;
		public List<TaskEditField> Fields = new();

        public TaskEditField GetEditField(string fieldName)
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                var fieldInfo = Fields[i];
                if (fieldInfo.FieldName == fieldName)
                {
                    return fieldInfo;
                }
            };
            return null;
        }
    }

	public partial class TaskTimelineEditData : TaskEditData
	{
		public float StartTime;
		public float Duration;
	}
    #endregion

    /// <summary>
    /// Data that exists only when editing tasks. Just like UIModel.
    /// </summary>
    public static class EditorModel
	{
        #region Common

        #region Lifecycle
		public static void OnReady()
		{

		}

		public static void OnProcess(double delta)
		{

		}
        #endregion

        #region Context Type
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
                        OnBindingContextTypeChanged?.Invoke();
					}
				}
			}
		}
		public static TaskContextExportInfo BindingContextInfo => m_BindingContextInfo;
		public static Action OnBindingContextTypeChanged;
        #endregion

        #region Current Select Task Node
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
		#endregion

		#endregion

		#region Timeline
		public static Timeline TimelineData;
		public class Timeline
		{
			#region Task List
			private List<string> m_TaskTypes = new();
			public List<string> TaskTypes
			{
				get
				{
					return m_TaskTypes;
				}
			}
			public static Action OnTaskTypesChanged;
            #endregion
        }
        #endregion

        #region Node Graph
        public static NodeGraph NodeGraphData;
		public class NodeGraph
		{

		}
        #endregion
    }
}
