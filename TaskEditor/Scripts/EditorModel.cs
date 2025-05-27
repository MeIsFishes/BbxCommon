using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    #region Base Struct
	// There are two kinds of data: TaskExport and TaskEdit.
	// Classes namely begin with TaskExport: Exported from Unity project, which is originally Task definition.
	// Classes namely begin with TaskEdit: Record data in TaskEditor, and prepare to convert to TaskValueInfo.
	// TaskValueInfo: For assigning values to Tasks. It's for constructing Tasks during runtime.

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
		public Action OnStartTimeChanged;
		public Action OnDurationChanged;

		private float m_StartTime;
		public float StartTime
		{
			get => m_StartTime;
			set
			{
				if (m_StartTime != value)
				{
					m_StartTime = value;
					OnStartTimeChanged?.Invoke();
				}
			}
		}

		private float m_Duration;
		public float Duration
		{
			get => m_Duration;
			set
			{
				if (m_Duration != value)
				{
					m_Duration = value;
					OnDurationChanged?.Invoke();
				}
			}
		}
	}
    #endregion

    /// <summary>
    /// Data that exists only when editing tasks. Just like UIModel.
	/// If you need event/message, see <see cref="EventBus"/>.
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
					EventBus.DispatchEvent(EEvent.CurSelectTaskNodeChanged);
                }
			}
		}
		#endregion

		#endregion

		#region Timeline
		public static Timeline TimelineData = new();
		public class Timeline
		{
			#region Task List
			public List<TimelineNode> Nodes = new();
			public List<TaskTimelineEditData> TaskDatas = new();

			private float m_MaxTime;
			public float MaxTime
			{
				get => m_MaxTime;
				set
				{
					if (m_MaxTime != value)
					{
						m_MaxTime = value;
						EventBus.DispatchEvent(EEvent.TimelineMaxTimeChanged);
					}
				}
			}
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
