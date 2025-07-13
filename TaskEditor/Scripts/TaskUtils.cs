using BbxCommon.Internal;
using Godot;
using System;
namespace BbxCommon
{
	public static class TaskUtils
	{
        #region Common
        public static string GetTaskDisplayName(string taskType)
		{
            taskType = taskType.TryRemoveStart("TaskNode");
            taskType = taskType.TryRemoveStart("Task");
			return taskType;
        }

        public static bool IsEnum(string typeName)
        {
            return EditorDataStore.IsEnum(typeName);
        }

		public static TaskEnumExportInfo GetEnumInfo(string typeName)
		{
			return EditorDataStore.GetEnumInfo(typeName);
		}

		public static TaskEnumExportInfo GetEnumInfo(TaskExportTypeInfo typeInfo)
		{
			return EditorDataStore.GetEnumInfo(typeInfo.TypeName);
		}

        public static void SetEnumPresetValues(OptionButton option, TaskEnumExportInfo enumInfo)
        {
            option.Clear();
            for (int i = 0; i < enumInfo.EnumValues.Count; i++)
            {
                option.AddItem(enumInfo.EnumValues[i]);
            }
        }
        #endregion

        #region Convert
        public static TimelineItemEditData TaskExportInfoToTimelineEditData(TaskExportInfo exportInfo)
        {
            var editData = new TimelineItemEditData();
            editData.TaskType = exportInfo.TaskTypeName;
            editData.Fields.Clear();
            for (int i = 0; i < exportInfo.FieldInfos.Count; i++)
            {
                var fieldInfo = exportInfo.FieldInfos[i];
                var editField = new TaskEditField();
                editField.FieldName = fieldInfo.FieldName;
                editField.TypeInfo = fieldInfo.TypeInfo;
                editField.Value = string.Empty;
                editData.Fields.Add(editField);
            }
            return editData;
        }

        public static TaskValueInfo TaskEditDataToTaskValueInfo(TaskEditData editData)
        {
            var taskExportInfo = EditorDataStore.GetTaskInfo(editData.TaskType);
            if (taskExportInfo == null)
            {
                DebugApi.LogError("Cannot find the TaskExportInfo for TaskEditData: " + editData.TaskType);
                return null;
            }
            var valueInfo = new TaskValueInfo();
            valueInfo.FullTypeName = taskExportInfo.TaskFullTypeName;
            for (int i = 0; i < editData.Fields.Count; i++)
            {
                var field = editData.Fields[i];
                valueInfo.AddFieldInfo(field.FieldName, field.ValueSource, field.Value);
            }
            return valueInfo;
        }
        #endregion

        #region Extension
        public static bool IsEnum(this TaskExportTypeInfo typeInfo)
        {
            return EditorDataStore.IsEnum(typeInfo);
        }

        public static bool IsList(this TaskExportTypeInfo typeInfo)
		{
			return typeInfo.TypeName == "List";
		}
        #endregion
    }
}
