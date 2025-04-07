using System;
using System.Collections.Generic;
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

        #region Export Task
        internal static TaskExportTypeInfo GenerateTaskTypeInfo(Type type)
        {
            var res = new TaskExportTypeInfo();
            if (type == typeof(bool))
                res.TypeName = "bool";
            else if (type == typeof(char))
                res.TypeName = "char";
            else if (type == typeof(short))
                res.TypeName = "short";
            else if (type == typeof(ushort))
                res.TypeName = "ushort";
            else if (type == typeof(int))
                res.TypeName = "int";
            else if (type == typeof(uint))
                res.TypeName = "uint";
            else if (type == typeof(long))
                res.TypeName = "long";
            else if (type == typeof(ulong))
                res.TypeName = "ulong";
            else if (type == typeof(float))
                res.TypeName = "float";
            else if (type == typeof(double))
                res.TypeName = "double";
            else if (type == typeof(string))
                res.TypeName = "string";
            else if (type.IsSubclassOf(typeof(List<>)))
            {
                res.TypeName = "List";
                res.GenericType1 = GenerateTaskTypeInfo(type.GenericTypeArguments[0]);
            }
            else if (type.IsSubclassOf(typeof(HashSet<>)))
            {
                res.TypeName = "HashSet";
                res.GenericType1 = GenerateTaskTypeInfo(type.GenericTypeArguments[0]);
            }
            else if (type.IsSubclassOf(typeof(SerializableHashSet<>)))
            {
                res.TypeName = "SerializableHashSet";
                res.GenericType1 = GenerateTaskTypeInfo(type.GenericTypeArguments[0]);
            }
            else if (type.IsSubclassOf(typeof(Dictionary<,>)))
            {
                res.TypeName = "Dictionary";
                res.GenericType1 = GenerateTaskTypeInfo(type.GenericTypeArguments[0]);
                res.GenericType2 = GenerateTaskTypeInfo(type.GenericTypeArguments[1]);
            }
            else if (type.IsSubclassOf(typeof(SerializableDic<,>)))
            {
                res.TypeName = "SerializableDic";
                res.GenericType1 = GenerateTaskTypeInfo(type.GenericTypeArguments[0]);
                res.GenericType2 = GenerateTaskTypeInfo(type.GenericTypeArguments[1]);
            }
            else if (type.IsEnum)
            {
                res.TypeName = type.FullName;
                if (ExportTaskInfo.EnumDic.ContainsKey(type.FullName) == false)
                {
                    ExportTaskInfo.EnumDic.Add(type.FullName, type);
                }
            }
            else
            {
                res.TypeName = type.Name;
            }
            return res;
        }
        #endregion
    }
}
