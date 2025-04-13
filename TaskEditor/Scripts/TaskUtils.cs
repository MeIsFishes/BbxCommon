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
