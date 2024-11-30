using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

namespace BbxCommon
{
    public static class JsonApi
    {
        private static string m_TypeInfoKey = "Default.TypeInfo";
        private static string m_FullTypeKey = "FullType";
        private static string m_TypeWithAssemblyKey = "AssemblyQualifiedName";
        private static string m_SpecialTypeKey = "SpecialType";
        private static string m_GenericType1Key = "GenericType1";
        private static string m_GenericType2Key = "GenericType2";

        #region Serialize

        #region Body
        public static JsonData Serialize(object obj)
        {
            try
            {
                var jsonData = ConvertObjectToJsonData(obj);
                return jsonData;
            }
            catch (Exception e)
            {
                DebugApi.LogError("Json serialization of " + obj.GetType().FullName + " failed!");
                DebugApi.LogException(e);
                return null;
            }
        }

        public static JsonData Serialize(object obj, string absolutePath)
        {
            JsonData jsonData = null;
            StreamWriter streamWriter = null;
            try
            {
                absolutePath = FileApi.AddExtensionIfNot(absolutePath, ".json");
                jsonData = ConvertObjectToJsonData(obj);
                var jsonWriter = new JsonWriter(new StringBuilder());
                jsonWriter.PrettyPrint = true;
                jsonWriter.IndentValue = 4;
                JsonMapper.ToJson(jsonData, jsonWriter);
                FileApi.CreateAbsolutePathFile(absolutePath);
                streamWriter = new StreamWriter(absolutePath);
                streamWriter.Write(jsonWriter.TextWriter.ToString());
                DebugApi.Log("Serialized the current file to " + absolutePath + ".");
                return jsonData;
            }
            catch (Exception e)
            {
                DebugApi.LogError("Json serialization exports " + obj.GetType().FullName + " to " + absolutePath + " failed!");
                DebugApi.LogException(e);
                return null;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
        }

        private static JsonData ConvertObjectToJsonData(object obj)
        {
            if (obj == null)
            {
                return new JsonData("null");
            }
            if (obj is Boolean booleanObj)
            {
                return new JsonData((bool)booleanObj);
            }
            if (obj is Int32 int32Obj)
            {
                return new JsonData((int)int32Obj);
            }
            if (obj is Int64 int64Obj)
            {
                return new JsonData((long)int64Obj);
            }
            if (obj is Single singleObj)
            {
                return new JsonData((double)singleObj);
            }
            if (obj is Double doubleObj)
            {
                return new JsonData((double)doubleObj);
            }
            if (obj is String stringObj)
            {
                return new JsonData((string)stringObj);
            }
            if (obj is Enum enumObj)
            {
                var enumJsonData = new JsonData();
                enumJsonData[m_TypeInfoKey] = new JsonData();
                enumJsonData[m_TypeInfoKey][m_FullTypeKey] = new JsonData(enumObj.GetType().FullName);
                enumJsonData[m_TypeInfoKey][m_TypeWithAssemblyKey] = new JsonData(enumObj.GetType().AssemblyQualifiedName);
                enumJsonData["Value"] = new JsonData(Enum.GetName(enumObj.GetType(), obj));
                return enumJsonData;
            }
            // special types
            var type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return ConvertListToJsonData(obj, type);
            }
            // serialize class
            var jsonData = new JsonData();
            jsonData[m_TypeInfoKey] = GenerateTypeInfo(type);
            foreach (var field in type.GetFields())
            {
                var value = field.GetValue(obj);
                jsonData[field.Name] = ConvertObjectToJsonData(value);
            }
            return jsonData;
        }
        #endregion

        #region Special Types
        private static JsonData ConvertListToJsonData(object obj, Type type)
        {
            var listJsonData = new JsonData();
            listJsonData[m_TypeInfoKey] = GenerateTypeInfo(type);
            var enumerator = obj as IEnumerable;
            int index = 0;
            foreach (var item in enumerator)
            {
                listJsonData[index.ToString()] = ConvertObjectToJsonData(item);
                index++;
            }
            return listJsonData;
        }
        #endregion

        #region Type Info
        private static JsonData GenerateTypeInfo(Type type)
        {
            var jsonData = new JsonData();
            // special types
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    jsonData[m_SpecialTypeKey] = new JsonData("List");
                    jsonData[m_GenericType1Key] = GenerateTypeInfo(type.GetGenericArguments()[0]);
                }
            }
            else
            {
                if (type == typeof(bool))
                    jsonData[m_SpecialTypeKey] = new JsonData("bool");
                else if (type == typeof(byte))
                    jsonData[m_SpecialTypeKey] = new JsonData("byte");
                else if (type == typeof(short))
                    jsonData[m_SpecialTypeKey] = new JsonData("short");
                else if (type == typeof(ushort))
                    jsonData[m_SpecialTypeKey] = new JsonData("ushort");
                else if (type == typeof(int))
                    jsonData[m_SpecialTypeKey] = new JsonData("int");
                else if (type == typeof(uint))
                    jsonData[m_SpecialTypeKey] = new JsonData("uint");
                else if (type == typeof(long))
                    jsonData[m_SpecialTypeKey] = new JsonData("long");
                else if (type == typeof(ulong))
                    jsonData[m_SpecialTypeKey] = new JsonData("ulong");
                else if (type == typeof(float))
                    jsonData[m_SpecialTypeKey] = new JsonData("float");
                else if (type == typeof(double))
                    jsonData[m_SpecialTypeKey] = new JsonData("double");
                else if (type == typeof(string))
                    jsonData[m_SpecialTypeKey] = new JsonData("string");
                else
                {
                    jsonData[m_FullTypeKey] = type.FullName;
                    jsonData[m_TypeWithAssemblyKey] = type.AssemblyQualifiedName;
                }
            }
            return jsonData;
        }
        #endregion

        #endregion

        #region Deserialize

        #region Body
        public static object Deserialize(JsonData jsonData)
        {
            object res = null;
            try
            {
                res = ConvertJsonDataToObject(jsonData);
                return res;
            }
            catch (Exception e)
            {
                DebugApi.LogError("Json deserialization failed!");
                DebugApi.LogException(e);
                return null;
            }
        }

        public static object Deserialize(string absolutePath)
        {
            StreamReader streamReader = null;
            object res = null;
            try
            {
                absolutePath = FileApi.AddExtensionIfNot(absolutePath, ".json");
                streamReader = new StreamReader(absolutePath);
                var jsonString = streamReader.ReadToEnd();
                var jsonData = JsonMapper.ToObject(jsonString);
                res = Deserialize(jsonData);
                return res;
            }
            catch (Exception e)
            {
                DebugApi.LogException(e);
                return null;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                if (res == null)
                    DebugApi.LogError("Json deserialization failed! File path: " + absolutePath);
            }
        }

        public static T Deserialize<T>(JsonData jsonData)
        {
            var obj = Deserialize(jsonData);
            if (obj == null)
            {
                return default;
            }
            if (obj is T t)
            {
                return t;
            }
            else
            {
                DebugApi.LogError("Json deserialization succeeded, but its type is " + obj.GetType().FullName + ", as you require " + typeof(T).FullName + ".");
                return default;
            }
        }

        public static T Deserialize<T>(string absolutePath)
        {
            var obj = Deserialize(absolutePath);
            if (obj == null)
            {
                return default;
            }
            if (obj is T t)
            {
                return t;
            }
            else
            {
                DebugApi.LogError("Json deserialization succeeded, but its type is " + obj.GetType().FullName + ", as you require " + typeof(T).FullName + ".");
                return default;
            }
        }

        private static object ConvertJsonDataToObject(JsonData jsonData)
        {
            if (jsonData.GetJsonType() == JsonType.Object && jsonData.ContainsKey(m_TypeInfoKey))
            {
                Type type = DeserializeTypeInfo(jsonData[m_TypeInfoKey]);
                if (type.IsEnum)
                {
                    var enumValue = Enum.Parse(type, (string)jsonData["Value"]);
                    return enumValue;
                }
                // special types
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return ConvertJsonDataToList(jsonData, type);
                }
                else
                {
                    var obj = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (var key in jsonData.Keys)
                    {
                        if (key == m_TypeInfoKey)
                            continue;
                        var field = type.GetField(key);
                        var value = ConvertJsonDataToObject(jsonData[key]);
                        var finalValue = Convert.ChangeType(value, field.FieldType);
                        field.SetValue(obj, finalValue);
                    }
                    return obj;
                }
            }
            switch (jsonData.GetJsonType())
            {
                case JsonType.Boolean:
                    return (bool)jsonData;
                case JsonType.Int:
                    return (int)jsonData;
                case JsonType.Long:
                    return (long)jsonData;
                case JsonType.Double:
                    return (double)jsonData;
                case JsonType.String:
                    if ((string)jsonData == "null")
                        return null;
                    else
                        return (string)jsonData;
            }
            return null;
        }
        #endregion

        #region Special Types
        private static object ConvertJsonDataToList(JsonData jsonData, Type type)
        {
            var list = Activator.CreateInstance(type);
            var addMethod = type.GetMethod("Add");
            int index = 0;
            while (jsonData.ContainsKey(index.ToString()))
            {
                var element = ConvertJsonDataToObject(jsonData[index.ToString()]);
                addMethod.Invoke(list, new object[] { element });
                index++;
            }
            return list;
        }
        #endregion

        #region Type Info
        private static Type DeserializeTypeInfo(JsonData jsonData)
        {
            if (jsonData.ContainsKey(m_SpecialTypeKey))
            {
                Type type = null;
                switch ((string)jsonData[m_SpecialTypeKey])
                {
                    case "bool":
                        return typeof(bool);
                    case "byte":
                        return typeof(byte);
                    case "short":
                        return typeof(short);
                    case "ushort":
                        return typeof(ushort);
                    case "int":
                        return typeof(int);
                    case "uint":
                        return typeof(uint);
                    case "long":
                        return typeof(long);
                    case "ulong":
                        return typeof(ulong);
                    case "float":
                        return typeof(float);
                    case "double":
                        return typeof(double);
                    case "string":
                        return typeof(string);
                    case "List":
                        type = typeof(List<>);
                        type = type.MakeGenericType(DeserializeTypeInfo(jsonData[m_GenericType1Key]));
                        return type;
                }
            }
            else if (jsonData.ContainsKey(m_FullTypeKey) && jsonData.ContainsKey(m_TypeWithAssemblyKey))
            {
                var fullType = (string)jsonData[m_FullTypeKey];
                var typeWithAssembly = (string)jsonData[m_TypeWithAssemblyKey];
                return ReflectionApi.GetType(fullType, typeWithAssembly);
            }
            return null;
        }
        #endregion

        #endregion
    }
}
