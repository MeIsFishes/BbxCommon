using System;
using BbxCommon.Internal;

namespace BbxCommon
{
    public static class TaskApi
    {
        #region Task Store
        public static void RegisterTask(string key, TaskGroupInfo value)
        {
            TaskManager.Instance.RegisterTask(key, value);
        }

        public static void RunTask(TaskBase task)
        {
            TaskManager.Instance.RunTask(task);
        }

        public static void RunTask(string key, TaskContextBase context)
        {
            TaskManager.Instance.RunTask(key, context);
        }
        #endregion

        #region Create Task by Code
        public static TaskGroupInfo CreateTaskInfo<T>(string key, int rootId) where T : TaskContextBase
        {
            var groupInfo = new TaskGroupInfo();
            groupInfo.BindingContextFullType = typeof(T).FullName;
            groupInfo.RootTaskId = rootId;
            TaskManager.Instance.RegisterTask(key, groupInfo);
            return groupInfo;
        }
        #endregion
    }
}
