using System;
using System.Text;
using System.IO;
using LitJson;

namespace BbxCommon
{
    public static class JsonApi
    {
        public static void Serialize(object obj, string path)
        {
            try
            {
                var jsonData = AsJsonData(obj);
                var jsonWriter = new JsonWriter(new StringBuilder());
                jsonWriter.PrettyPrint = true;
                jsonWriter.IndentValue = 4;
                JsonMapper.ToJson(jsonData, jsonWriter);
                FileApi.CreateAbsolutePathFile(path);
                var streamWriter = new StreamWriter(path);
                streamWriter.Write(jsonWriter.TextWriter.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch (Exception e)
            {
                DebugApi.LogError(e);
                return;
            }
            DebugApi.Log("Serialized the current file to " + path);
        }

        private static JsonData AsJsonData(object obj)
        {
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
                enumJsonData["Default.FullType"] = new JsonData(enumObj.GetType().FullName);
                enumJsonData["Value"] = new JsonData(Enum.GetName(enumObj.GetType(), obj));
                return enumJsonData;
            }
            // serialize class
            var jsonData = new JsonData();
            var type = obj.GetType();
            jsonData["Default.FullType"] = new JsonData(type.FullName);
            foreach (var field in type.GetFields())
            {
                var value = field.GetValue(obj);
                jsonData[field.Name] = AsJsonData(value);
            }
            return jsonData;
        }
    }
}
