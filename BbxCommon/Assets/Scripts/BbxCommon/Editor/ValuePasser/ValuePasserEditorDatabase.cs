using System;
using System.Reflection;
using System.Collections.Generic;

namespace BbxCommon.ValuePasserInternal
{
    public class ValuePasserEditorDatabase
    {
        /// <summary>
        /// Dictionary of the VariableType and the VariableDataType
        /// </summary>
        public static Dictionary<Type, Type> VariableDatas
        {
            get
            {
                if (m_VariableDatas == null)
                    m_VariableDatas = LoadAllVariableDatas();
                return m_VariableDatas;
            }
        }
        private static Dictionary<Type, Type> m_VariableDatas;

        private static Dictionary<Type, Type> LoadAllVariableDatas()
        {
            Dictionary<Type, Type> typeDic = new Dictionary<Type, Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // get all types through reflection
            foreach (Assembly assembly in assemblies)
            {
                foreach (var classType in assembly.GetTypes())
                {
                    if (classType.IsAbstract == false)
                    {
                        var dataAttr = classType.GetCustomAttribute<VariableDataAttribute>();
                        foreach (var variableType in dataAttr.VariableTypes)
                        {
                            if (typeDic.ContainsKey(variableType))
                                DebugApi.LogError("ValuePasserDataBase: Trying register " + classType.Name + " to " + variableType.Name + " has failed!" +
                                    " The VariableData " + typeDic[variableType].Name + " has already registered!");
                            else
                                typeDic.Add(variableType, classType);
                        }
                    }
                }
            }
            return typeDic;
        }
    }
}
