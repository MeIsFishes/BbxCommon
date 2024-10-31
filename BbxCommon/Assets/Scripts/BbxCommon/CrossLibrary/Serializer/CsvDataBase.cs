using System;
using System.Collections.Generic;
using System.IO;
using BbxCommon.Internal;

namespace BbxCommon
{
    public abstract class CsvDataBase<T> : CsvDataBase where T : CsvDataBase<T>, new()
    {
        #region Body
        public override sealed void ReadFromAbsolutePath(string absolutePath)
        {
            ReadWithAbsolutePath<T>(absolutePath);
        }

        public override sealed void ReadFromString(string filePath, string content)
        {
            ReadFromString<T>(filePath, content);
        }
        #endregion
    }
}

namespace BbxCommon.Internal
{
    public abstract class CsvDataBase
    {
        #region Body
        public enum EDataLoad
        {
            Addition,
            Override,
        }

        public virtual EDataLoad GetDataLoadType() { return EDataLoad.Addition; }
        public virtual string GetDataGroup() { return "GameEngineDefault"; }
        public abstract string[] GetTableNames();

        internal static List<string> CsvKeys = new List<string>();
        internal static Dictionary<string, string> KeyValuePairs = new Dictionary<string, string>();

        // dynamic data
        internal static string CurrentPath;
        internal static int CurrentLineIndex;

        internal static void ReadWithAbsolutePath<T>(string absolutePath) where T : CsvDataBase<T>, new()
        {
            CurrentPath = FileApi.AddExtensionIfNot(absolutePath, ".csv");
            CsvKeys.Clear();
            KeyValuePairs.Clear();
            var streamReader = new StreamReader(CurrentPath);

            // read keys
            var firstLine = streamReader.ReadLine().TryRemoveEnd("\r");
            var keys = firstLine.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                CsvKeys.Add(keys[i]);
            }

            CurrentLineIndex = 1;
            while (true)
            {
                CurrentLineIndex++;
                var lineStr = streamReader.ReadLine().TryRemoveEnd("\r");
                if (lineStr == null)
                    break;

                if (SplitLineIntoKeyValue(lineStr) == false)
                    continue;

                var csvInstance = new T();
                try
                {
                    csvInstance.ReadLine();
                }
                catch (Exception e)
                {
                    DebugApi.LogError(e);
                }
            }

            streamReader.Close();
        }

        /// <summary>
        /// Read from a standard CSV string.
        /// </summary>
        /// <param name="filePath"> Use for logging broken file. </param>
        /// <param name="content"> The complete CSV string. </param>
        internal static void ReadFromString<T>(string filePath, string content) where T : CsvDataBase<T>, new()
        {
            CurrentPath = FileApi.AddExtensionIfNot(filePath, ".csv");
            CsvKeys.Clear();
            KeyValuePairs.Clear();
            var lines = content.SplitIntoLines();
            if (lines.Length == 0)
            {
                DebugApi.LogError("Invalid CSV string! The file path you pass in is " +  filePath);
                return;
            }

            // read keys
            var firstLine = lines[0];
            var keys = firstLine.Split(',');
            for (int i = 0; i < keys.Length; i++)
            {
                CsvKeys.Add(keys[i]);
            }

            CurrentLineIndex = 1;
            for (int i = 1; i < lines.Length; i++)
            {
                CurrentLineIndex++;
                if (SplitLineIntoKeyValue(lines[i]) == false)
                    continue;

                var csvInstance = new T();
                try
                {
                    csvInstance.ReadLine();
                }
                catch (Exception e)
                {
                    DebugApi.LogError(e);
                }
            }
        }

        /// <summary>
        /// True means split successfully, false means skip this line.
        /// </summary>
        private static bool SplitLineIntoKeyValue(string lineStr)
        {
            if (lineStr == null)
                return false;

            if (lineStr.StartsWith("//"))   // line starts with "//" will be recognized as comment
                return false;

            var lineValues = lineStr.Split(',');
            if (lineValues.Length != CsvKeys.Count)
            {
                DebugApi.LogError("Broken line in " + CurrentPath + ", line index: " + CurrentLineIndex + ". Are there invalid commas?");
                return false;
            }
            for (int i = 0; i < lineValues.Length; i++)
            {
                KeyValuePairs[CsvKeys[i]] = lineValues[i];
            }
            return true;
        }

        protected abstract void ReadLine();
        public abstract void ReadFromAbsolutePath(string absolutePath);
        public abstract void ReadFromString(string filePath, string content);
        #endregion

        #region Parse

        #region Parse Single Object
        protected string GetStringFromKey(string key)
        {
            return KeyValuePairs[key];
        }

        private bool ParseBool(string str, out bool result)
        {
            bool succeeded = false;
            if (str == "0" || str.ToUpper() == "FALSE")
            {
                succeeded = true;
                result = false;
            }
            else if (str == "1" || str.ToUpper() == "TRUE")
            {
                succeeded = true;
                result = true;
            }
            else
            {
                succeeded = false;
                result = false;
            }
            return succeeded;
        }

        protected bool ParseBoolFromKey(string key, bool defaultValue = false)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (ParseBool(str, out var result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: bool");
                return defaultValue;
            }
        }

        protected int ParseIntFromKey(string key, int defaultValue = 0)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: int");
                return defaultValue;
            }
        }

        protected uint ParseUintFromKey(string key, uint defaultValue = 0)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (uint.TryParse(str, out uint result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: uint");
                return defaultValue;
            }
        }

        protected long ParseLongFromKey(string key, long defaultValue = 0)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (long.TryParse(str, out long result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: long");
                return defaultValue;
            }
        }

        protected ulong ParseUlongFromKey(string key, ulong defaultValue = 0)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (ulong.TryParse(str, out ulong result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: ulong");
                return defaultValue;
            }
        }

        protected float ParseFloatFromKey(string key, float defaultValue = 0)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (float.TryParse(str, out float result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: float");
                return defaultValue;
            }
        }

        protected double ParseDoubleFromKey(string key, double defaultValue)
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (double.TryParse(str, out double result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: double");
                return defaultValue;
            }
        }

        protected T ParseEnumFromKey<T>(string key, T defaultValue = default) where T : unmanaged, Enum
        {
            var str = KeyValuePairs[key];
            if (str == "")
            {
                return defaultValue;
            }
            if (Enum.TryParse(str, true, out T result))
            {
                return result;
            }
            else
            {
                DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: " + typeof(T).FullName);
                return defaultValue;
            }
        }
        #endregion

        #region Parse Array
        /// <param name="ignoreSpace"> If true, all spaces will be removed. That means you can write array like "1; 2; 3". </param>
        private string[] SplitFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var str = KeyValuePairs[key];
            if (ignoreSpace)
                str = str.Replace(" ", "");
            return str.Split(separators);
        }

        protected string[] GetStringArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            return SplitFromKey(key, ignoreSpace, separators);
        }

        protected string[] GetStringArrayFromKey(string key)
        {
            return SplitFromKey(key, false, ';');
        }

        protected bool[] ParseBoolArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            bool[] result = new bool[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (ParseBool(strs[i], out var boolResult))
                {
                    result[i] = boolResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: BoolArray");
                    return new bool[0];
                }
            }
            return result;
        }

        protected bool[] ParseBoolArrayFromKey(string key)
        {
            return ParseBoolArrayFromKey(key, true, ';');
        }

        protected int[] ParseIntArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            int[] result = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (int.TryParse(strs[i], out var intResult))
                {
                    result[i] = intResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: IntArray");
                    return new int[0];
                }
            }
            return result;
        }

        protected int[] ParseIntArrayFromKey(string key)
        {
            return ParseIntArrayFromKey(key, true, ';');
        }

        protected uint[] ParseUintArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            uint[] result = new uint[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (uint.TryParse(strs[i], out var uintResult))
                {
                    result[i] = uintResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: UIntArray");
                    return new uint[0];
                }
            }
            return result;
        }

        protected uint[] ParseUintArrayFromKey(string key)
        {
            return ParseUintArrayFromKey(key, true, ';');
        }

        protected long[] ParseLongArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            long[] result = new long[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (long.TryParse(strs[i], out var longResult))
                {
                    result[i] = longResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: LongArray");
                    return new long[0];
                }
            }
            return result;
        }

        protected long[] ParseLongArrayFromKey(string key)
        {
            return ParseLongArrayFromKey(key, true, ';');
        }

        protected ulong[] ParseUlongArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            ulong[] result = new ulong[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (ulong.TryParse(strs[i], out var ulongResult))
                {
                    result[i] = ulongResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: ULongArray");
                    return new ulong[0];
                }
            }
            return result;
        }

        protected ulong[] ParseUlongArrayFromKey(string key)
        {
            return ParseUlongArrayFromKey(key, true, ';');
        }

        protected float[] ParseFloatArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            float[] result = new float[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (float.TryParse(strs[i], out var floatResult))
                {
                    result[i] = floatResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: FloatArray");
                    return new float[0];
                }
            }
            return result;
        }

        protected float[] ParseFloatArrayFromKey(string key)
        {
            return ParseFloatArrayFromKey(key, true, ';');
        }

        protected double[] ParseDoubleArrayFromKey(string key, bool ignoreSpace, params char[] separators)
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            double[] result = new double[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (double.TryParse(strs[i], out var doubleResult))
                {
                    result[i] = doubleResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: DoubleArray");
                    return new double[0];
                }
            }
            return result;
        }

        protected double[] ParseDoubleArrayFromKey(string key)
        {
            return ParseDoubleArrayFromKey(key, true, ';');
        }

        protected T[] ParseEnumArrayFromKey<T>(string key, bool ignoreSpace, params char[] separators) where T : unmanaged, Enum
        {
            var strs = SplitFromKey(key, ignoreSpace, separators);
            T[] result = new T[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                if (Enum.TryParse<T>(strs[i], true, out var enumResult))
                {
                    result[i] = enumResult;
                }
                else
                {
                    DebugApi.LogError("Broken CSV cell!\nfile path: " + CurrentPath + "\nline: " + CurrentLineIndex + "\nkey: " + key + "\nrequire type: Array of " + typeof(T).FullName);
                    return new T[0];
                }
            }
            return result;
        }

        protected T[] ParseEnumArrayFromKey<T>(string key) where T : unmanaged, Enum
        {
            return ParseEnumArrayFromKey<T>(key, true, ';');
        }
        #endregion

        #endregion
    }
}
