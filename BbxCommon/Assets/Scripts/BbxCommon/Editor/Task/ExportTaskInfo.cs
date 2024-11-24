using System;
using System.IO;
using UnityEditor;

namespace BbxCommon
{
    public static class ExportTaskInfo
    {
        private static string m_ExportPath = "../ExportedTaskInfo/";

        [MenuItem("BbxCommon/ExportAllTasks")]
        public static void ExportAllTasks()
        {
            var fullPath = Path.GetFullPath(m_ExportPath);
            FileApi.CreateAbsoluteDirectory(fullPath);
            foreach (var path in FileApi.SearchAllFilesInFolder(fullPath))
            {
                File.Delete(path);
            }
            foreach (var type in ReflectionApi.GetAllTypesEnumerator())
            {
                if (type.IsClass &&
                    type.IsAbstract == false &&
                    type.IsSubclassOf(typeof(TaskBase)))
                {
                    var task = Activator.CreateInstance(type) as TaskBase;
                    var taskExportInfo = task.GenerateExportInfo();
                    JsonApi.Serialize(taskExportInfo, fullPath + type.Name + ".json");
                }
                else if (type.IsClass &&
                    type.IsAbstract == false &&
                    type.IsSubclassOf(typeof(TaskContextBase)))
                {
                    var taskContext = Activator.CreateInstance(type) as TaskContextBase;
                    var taskContextExportInfo = taskContext.GenerateExportInfo();
                    JsonApi.Serialize(taskContextExportInfo, fullPath + type.Name + ".json");
                }
            }
        }
    }
}
