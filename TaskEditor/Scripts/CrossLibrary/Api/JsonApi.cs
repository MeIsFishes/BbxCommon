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
        public static string FullTypeKey = "Default.FullType";
        public static string TypeWithAssembly = "Default.AssemblyQualifiedName";

        #region Serialize
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
                enumJsonData[FullTypeKey] = new JsonData(enumObj.GetType().FullName);
                enumJsonData[TypeWithAssembly] = new JsonData(enumObj.GetType().AssemblyQualifiedName);
                enumJsonData["Value"] = new JsonData(Enum.GetName(enumObj.GetType(), obj));
                return enumJsonData;
            }
            // special types
            var type = obj.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listJsonData = new JsonData();
                listJsonData[FullTypeKey] = new JsonData(obj.GetType().FullName);
                listJsonData[TypeWithAssembly] = new JsonData(obj.GetType().AssemblyQualifiedName);
                var enumerator = obj as IEnumerable;
                int index = 0;
                foreach (var item in enumerator)
                {
                    listJsonData[index.ToString()] = ConvertObjectToJsonData(item);
                    index++;
                }
                return listJsonData;
            }
            // serialize class
            var jsonData = new JsonData();
            jsonData[FullTypeKey] = new JsonData(type.FullName);
            jsonData[TypeWithAssembly] = new JsonData(type.AssemblyQualifiedName);
            foreach (var field in type.GetFields())
            {
                var value = field.GetValue(obj);
                jsonData[field.Name] = ConvertObjectToJsonData(value);
            }
            return jsonData;
        }
        #endregion

        #region Deserialize
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
            if (jsonData.GetJsonType() == JsonType.Object && jsonData.ContainsKey(FullTypeKey))
            {
                Type objType = null;
                if (jsonData.ContainsKey(TypeWithAssembly))
                {
                    objType = Type.GetType((string)jsonData[TypeWithAssembly]);
                }
                if (objType == null)
                {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var typeName = (string)jsonData[FullTypeKey];
                    foreach (var assembly in assemblies)
                    {
                        objType = assembly.GetType(typeName);
                        if (objType != null)
                            break;
                    }
                }
                if (objType.IsEnum)
                {
                    var enumValue = Enum.Parse(objType, (string)jsonData["Value"]);
                    return enumValue;
                }
                else
                {
                    var obj = objType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (var key in jsonData.Keys)
                    {
                        if (key != FullTypeKey)
                        {
                            var field = objType.GetField(key);
                            var value = ConvertJsonDataToObject(jsonData[key]);
                            var finalValue = Convert.ChangeType(value, field.FieldType);
                            field.SetValue(obj, finalValue);
                        }
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
                    return (string)jsonData;
            }
            return null;
        }
        #endregion
    }
}
