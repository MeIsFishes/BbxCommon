using BbxCommon.Internal;
using Godot;
using LitJson;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
    #region Base Struct
	// There are two kinds of data: TaskExport and TaskEdit.
	// Classes namely begin with TaskExport: Exported from Unity project, which is originally Task definition.
	// Classes namely begin with TaskEdit: Record data in TaskEditor, and prepare to convert to TaskValueInfo.
	// TaskValueInfo: For assigning values to Tasks. It's for constructing Tasks during runtime.

    public partial class TaskEditField
    {
        public string FieldName;
        public TaskExportTypeInfo TypeInfo;
        public ETaskFieldValueSource ValueSource;
        public string Value;
    }

	public partial class TaskEditData
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
			// hotkeys
            SimplePool.Alloc(out Dictionary<int, bool> keyStates);
			InputApi.GetKeyStateRequestDic(keyStates);
			foreach (var pair in keyStates)
			{
				if (Input.IsKeyPressed((Key)pair.Key))
				{
					keyStates[pair.Key] = true;
                }
			}
			InputApi.Tick(keyStates);
			keyStates.CollectToPool();
        }
        #endregion

        #region Variables and Callbacks

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

        #region Common
        public static TaskSelector TaskSelector;
		public static Inspector Inspector;
		public static SettingsPanel SettingsPanel;
		public static EditorRoot EditorRoot;
		public static TimelineRoot TimelineRoot;
        #endregion

        #region FileDialog
        private static FileDialog m_FileDialog;
		public static void OpenFileDialog(Action<string> selected, FileDialog.FileModeEnum fileModeEnum = FileDialog.FileModeEnum.OpenFile,
			string currentDir = null, string filter = null)
		{
			if (m_FileDialog == null)
			{
				m_FileDialog = new FileDialog();
				EditorRoot.AddChild(m_FileDialog);
			}
            m_FileDialog.Size = new Vector2I(600, 400);
            m_FileDialog.Position = new Vector2I(30, 60);
            m_FileDialog.Access = FileDialog.AccessEnum.Filesystem;
			m_FileDialog.FileMode = fileModeEnum;
			if (currentDir != null)
			{
				m_FileDialog.CurrentDir = FileApi.GetDirectory(currentDir);
            }
            switch (fileModeEnum)
			{
				case FileDialog.FileModeEnum.OpenFile:
				case FileDialog.FileModeEnum.SaveFile:
					m_FileDialog.FileSelected += (string s) => { selected(s); };
					break;
				case FileDialog.FileModeEnum.OpenDir:
                    m_FileDialog.DirSelected += (string s) => { selected(s); };
                    break;
				default:
					DebugApi.LogError("FileModeEnum you set is not supported: " + fileModeEnum);
					break;
			}
			if (filter != null)
				m_FileDialog.Filters = new string[1] { filter };
            m_FileDialog.Show();
        }
		#endregion

		#region AcceptDialog
		private static AcceptDialog m_AcceptDialog;

		public static void OpenAcceptDialog(string message)
		{
			OpenAcceptDialog("Notice", message);
		}

		public static void OpenAcceptDialog(string title, string message)
		{
			if (m_AcceptDialog == null)
			{
				m_AcceptDialog = new AcceptDialog();
                EditorRoot.AddChild(m_AcceptDialog);
            }
            m_AcceptDialog.Size = new Vector2I(600, 150);
            m_AcceptDialog.Position = new Vector2I(300, 350);
			m_AcceptDialog.DialogText = message;
			m_AcceptDialog.Title = title;
			m_AcceptDialog.Show();
        }
		#endregion

		#endregion

		#region Save Target

		#region Common
		public static List<SaveTargetBase> SaveTargetList = new(); // loaded files

		private static SaveTargetBase m_CurSaveTarget;
        public static SaveTargetBase CurSaveTarget
		{
			get
			{
				return m_CurSaveTarget;
			}
			set
			{
				if (m_CurSaveTarget != value)
				{
					m_CurSaveTarget = value;
                    EventBus.DispatchEvent(EEvent.CurSaveTargetChanged);
                }
			}
		}

		public abstract class SaveTargetBase
		{
			private string m_FilePath;
			public string FilePath
			{
				get => m_FilePath;
				set
				{
					m_FilePath = value.TryRemoveEnd(".editor.json", ".json");
				}
			}

            private string m_BindingContextType;
            public string BindingContextType
            {
                get => m_BindingContextType;
                set
                {
					if (m_BindingContextType != value)
					{
						m_BindingContextType = value;
					}
                }
            }
            public TaskContextExportInfo BindingContextInfo => EditorDataStore.GetTaskContextInfo(m_BindingContextType);

			public bool IsTimeline => this is TimelineSaveTargetData;
			public bool IsGraphNode => this is NodeGraphSaveTargetData;

            public abstract void Save(string filePath = null);
		}
        #endregion

        #region Timeline
        public static TimelineSaveTargetData TimelineSaveTarget => CurSaveTarget as TimelineSaveTargetData;
		public class TimelineSaveTargetData : SaveTargetBase
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

			public override void Save(string filePath)
			{
				try
				{
					// save editor file
					if (filePath.IsNullOrEmpty() == false)
					{
						FilePath = filePath;
					}
					var currentTaskPath = FilePath;
					var editorFilePath = currentTaskPath.TryRemoveEnd(".editor.json", ".json");
					editorFilePath += ".editor.json";
					JsonApi.Serialize(this, editorFilePath);
					// build timeline root info
					int taskId = 0;
					var taskGroupInfo = new TaskGroupInfo();
					var timelineRootValueInfo = new TaskValueInfo();
					timelineRootValueInfo.FullTypeName = EditorDataStore.GetTaskInfo("TaskTimeline").TaskFullTypeName;
					timelineRootValueInfo.AddFieldInfo("Duration", ETaskFieldValueSource.Value, MaxTime.ToString());
					taskGroupInfo.BindingContextFullType = BindingContextType;
					taskGroupInfo.SetRootTaskId(taskId);
					taskGroupInfo.TaskInfos[taskId++] = timelineRootValueInfo;
					// build task items
					foreach (var timelineItemEditData in TaskDatas)
					{
						// timeline item
						var itemInfo = new TaskTimelineItemInfo();
						var itemValueInfo = TaskUtils.TaskEditDataToTaskValueInfo(timelineItemEditData);
						taskGroupInfo.TaskInfos[taskId] = itemValueInfo;
						itemInfo.Id = taskId++;
						itemInfo.StartTime = timelineItemEditData.StartTime;
						itemInfo.Duration = timelineItemEditData.Duration;
						timelineRootValueInfo.AddTimelineInfo(timelineItemEditData.StartTime, timelineItemEditData.Duration, itemInfo.Id);
						foreach (var field in timelineItemEditData.Fields)
						{
							itemValueInfo.AddFieldInfo(field.FieldName, field.ValueSource, field.Value);
						}
						// enter condition
						foreach (var condition in timelineItemEditData.EnterConditions)
						{
							var conditionValueInfo = TaskUtils.TaskEditDataToTaskValueInfo(condition);
							taskGroupInfo.TaskInfos[taskId] = conditionValueInfo;
							itemValueInfo.AddEnterCondition(taskId++);
						}
						// condition
						foreach (var condition in timelineItemEditData.Conditions)
						{
							var conditionValueInfo = TaskUtils.TaskEditDataToTaskValueInfo(condition);
							taskGroupInfo.TaskInfos[taskId] = conditionValueInfo;
							itemValueInfo.AddCondition(taskId++);
						}
						// exit condition
						foreach (var condition in timelineItemEditData.ExitConditions)
						{
							var conditionValueInfo = TaskUtils.TaskEditDataToTaskValueInfo(condition);
							taskGroupInfo.TaskInfos[taskId] = conditionValueInfo;
							itemValueInfo.AddExitCondition(taskId++);
						}
					}
					JsonApi.Serialize(taskGroupInfo, currentTaskPath);
                    OpenAcceptDialog("Save Successfully!");
                }
				catch (Exception e)
				{
					OpenAcceptDialog("Error", e.Message);
				}
            }
        }
        #endregion

        #region Node Graph
        public static NodeGraphSaveTargetData NodeGraphSaveTarget => CurSaveTarget as NodeGraphSaveTargetData;
		public class NodeGraphSaveTargetData : SaveTargetBase
		{
            private string m_BindingContextType;

            public string GetBindingContextType()
            {
                return m_BindingContextType;
            }

            public void SetBindingContextType(string type)
            {
                m_BindingContextType = type;
            }

            public override void Save(string filePath)
			{

			}
		}
        #endregion

        #endregion
    }
}
