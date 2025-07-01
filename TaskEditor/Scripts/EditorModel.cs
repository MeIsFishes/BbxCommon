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

	public enum EEditingTaskType
	{
		Timeline,
		Graph,
	}

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

	public partial class TimelineItemEditData : TaskEditData
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

		public bool ExpandCondition = false;
		public List<TaskEditData> EnterConditions = new();
		public List<TaskEditData> Conditions = new();
		public List<TaskEditData> ExitConditions = new();
	}
    #endregion

    /// <summary>
    /// Data that exists only when editing tasks. It likes a mix UIModel and Facade.
	/// If you need event/message, see <see cref="EventBus"/>.
    /// </summary>
    public static class EditorModel
	{
        #region Lifecycle
		public static void OnReady()
		{
			EditorDataStore.DeserializeAllTaskInfo();
        }

		public static void OnProcess(double delta)
		{
            BbxButton.OnProcessHotkey();
        }
        #endregion

        #region Variables and Callbacks

        #region Editing Task
        private static EEditingTaskType m_EditingTaskType = EEditingTaskType.Timeline;
        public static EEditingTaskType EditingTaskType
		{
			get => m_EditingTaskType;
			set
			{
				if (m_EditingTaskType != value)
				{
					m_EditingTaskType = value;
					EventBus.DispatchEvent(EEvent.EdittingTaskTypeChanged);
				}
			}
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
					}
				}
			}
		}
		public static TaskContextExportInfo BindingContextInfo => m_BindingContextInfo;
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
					if (m_CurSelectTaskNode != null) m_CurSelectTaskNode.OnTaskSelected(false);
                    m_CurSelectTaskNode = value;
                    if (value != null) value.OnTaskSelected(true);
					EventBus.DispatchEvent(EEvent.CurSelectTaskNodeChanged);
                }
			}
		}
        #endregion

        #endregion

        #region UI Ref
		public static TaskSelector TaskSelector;
		public static Inspector Inspector;
		public static SettingsPanel SettingsPanel;
		public static EditorRoot EditorRoot;
		public static TimelineRoot TimelineRoot;

		private static bool m_FileDialogOpened;
		public static void OpenFileDialog(Action<string> selected, FileDialog.FileModeEnum fileModeEnum = FileDialog.FileModeEnum.OpenFile, string currentDir = null)
		{
			if (m_FileDialogOpened == true)
				return;

			var fileDialog = new FileDialog();
			fileDialog.Size = new Vector2I(600, 400);
			fileDialog.Position = new Vector2I(30, 60);
			fileDialog.Access = FileDialog.AccessEnum.Filesystem;
			fileDialog.FileMode = fileModeEnum;
			if (currentDir != null)
			{
				fileDialog.CurrentDir = currentDir;
            }
            switch (fileModeEnum)
			{
				case FileDialog.FileModeEnum.OpenFile:
				case FileDialog.FileModeEnum.SaveFile:
					fileDialog.FileSelected += (string s) => { selected(s); };
					break;
				case FileDialog.FileModeEnum.OpenDir:
                    fileDialog.DirSelected += (string s) => { selected(s); };
                    break;
				default:
					DebugApi.LogError("FileModeEnum you set is not supported: " + fileModeEnum);
					break;
			}
			fileDialog.CloseRequested += () => { m_FileDialogOpened = false; };
            fileDialog.Show();
			m_FileDialogOpened = true;
			EditorRoot.AddChild(fileDialog);
        }
        #endregion

        #region Save Target

        #region Common
		public static object SaveTarget
		{
			get
			{
				switch (EditingTaskType)
				{
                    case EEditingTaskType.Timeline:
                        return TimelineData;
                    case EEditingTaskType.Graph:
                        return NodeGraphData;
                    default:
                        DebugApi.LogError("Unknown EditingTaskType: " + EditingTaskType);
                        return null;
                }
			}
			set
			{
				switch (EditingTaskType)
				{
                    case EEditingTaskType.Timeline:
                        TimelineData = (TimelineDataStruct)value;
						break;
                    case EEditingTaskType.Graph:
                        NodeGraphData = (NodeGraphDataStruct)value;
						break;
                    default:
                        DebugApi.LogError("Unknown EditingTaskType: " + EditingTaskType);
						break;
                }
			}
		}
        #endregion

        #region Timeline
        public static TimelineDataStruct TimelineData = new();
		public class TimelineDataStruct
		{
			public List<TimelineItemEditData> TaskDatas = new();

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
        }
        #endregion

        #region Node Graph
        public static NodeGraphDataStruct NodeGraphData;
		public class NodeGraphDataStruct
		{

		}
        #endregion

        #endregion
    }
}
