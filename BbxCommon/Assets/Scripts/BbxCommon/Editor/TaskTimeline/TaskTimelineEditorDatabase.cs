using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BbxCommon.TaskInternal
{
    public class TaskTimelineEditorDatabase
    {
        public static Type[] Tasks
        {
            get
            {
                if (m_Tasks == null)
                    m_Tasks = LoadAllTasks();
                return m_Tasks;
            }
        }
        private static Type[] m_Tasks;

        private static Type[] LoadAllTasks()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // get all types through reflection
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    types.AddRange(assembly.GetTypes().Where(type => type.IsAbstract == false && typeof(TaskBase).IsAssignableFrom(type)).ToArray());
                }
                catch (ReflectionTypeLoadException) { }
            }
            types.Sort((Type t1, Type t2) => t1.Name.CompareTo(t2.Name));
            return types.ToArray();
        }
    }
}
