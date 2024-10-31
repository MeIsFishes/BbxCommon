using System;
using BbxCommon.Internal;

namespace BbxCommon
{
    public static class TaskApi
    {
        #region Create Task by Code
        public static TaskGroupInfo CreateTaskGroupInfo<T>() where T : TaskContextBase
        {
            var groupInfo = new TaskGroupInfo();
            groupInfo.BindingContextFullType = typeof(T).FullName;
            return groupInfo;
        }
        #endregion

        #region Internal
        /// <summary>
        /// Deserialize from <see cref="TaskValueInfo"/>.
        /// </summary>
        internal static TaskBase DeserializeTask(TaskValueInfo taskValueInfo, TaskContextBase taskContext)
        {
            var type = ReflectionApi.GetType(taskValueInfo.FullTypeName, taskValueInfo.AssemblyQualifiedName);
            if (type == null)
            {
                DebugApi.LogError("Invalid Task Type, FullTypeName = " + taskValueInfo.FullTypeName + ", AssemblyQualifiedName = " + taskValueInfo.AssemblyQualifiedName);
                return null;
            }
            var task = Activator.CreateInstance(type) as TaskBase;
            for (int i = 0; i < taskValueInfo.FieldInfos.Count; i++)
            {
                var fieldInfo = taskValueInfo.FieldInfos[i];
                var enumType = task.GetFieldEnumType();
                var enumValue = Enum.Parse(enumType, fieldInfo.Value);
                task.ReadFieldInfo(enumValue.GetHashCode(), fieldInfo, taskContext);
            }
            return task;
        }
        #endregion
    }
}
