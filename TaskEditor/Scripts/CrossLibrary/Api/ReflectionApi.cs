using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace BbxCommon
{
    /// <summary>
    /// <see cref="ReflectionApi"/> is just a tool class which wraps functions of C# reflection.
    /// </summary>
    public static class ReflectionApi
    {
        public static object Invoke<TClass>(TClass obj, string functionName)
        {
            var func = typeof(TClass).GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return func.Invoke(obj, new object[] { });
        }

        public static object Invoke<TClass>(TClass obj, string functionName, params object[] parameters)
        {
            var func = typeof(TClass).GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return func.Invoke(obj, parameters);
        }

        public static object InvokeStatic<TClass>(string functionName)
        {
            var func = typeof(TClass).GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return func.Invoke(null, new object[] { });
        }

        public static object InvokeStatic<TClass>(string functionName, params object[] parameters)
        {
            var func = typeof(TClass).GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            return func.Invoke(null, parameters);
        }

        public static object GetField<TClass>(TClass obj, string fieldName)
        {
            return typeof(TClass).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(obj);
        }

        public static void SetField<TClass>(TClass obj, string fieldName, object value)
        {
            typeof(TClass).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SetValue(obj, value);
        }

        private static Dictionary<string, Type> m_TypeDic = new();

        /// <summary>
        /// To ensure find out the unique result, you should pass in "BbxCommon.MyClass", but not "MyClass".
        /// </summary>
        public static Type GetType(string typeFullName)
        {
            if (m_TypeDic.TryGetValue(typeFullName, out var type))
            {
                return type;
            }
            else
            {
                type = Type.GetType(typeFullName);
                if (type == null)
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies)
                    {
                        type = assembly.GetType(typeFullName);
                        if (type != null)
                            break;
                    }
                    if (type == null)
                    {
                        foreach (var assembly in assemblies)
                        {
                            type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeFullName);
                            if (type != null)
                                break;
                        }
                    }
                }
                m_TypeDic.Add(typeFullName, type);
                return type;
            }
        }

        public static IEnumerable<Type> GetAllTypesEnumerator()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    yield return type;
                }
            }
        }
    }
}
