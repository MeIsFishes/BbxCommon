using BbxCommon.Internal;
using System.Collections.Generic;

namespace BbxCommon
{
    public static class CsvApi
    {
        public static void ReadWithAbsolutePath<T>(string absolutePath) where T : CsvDataBase<T>, new()
        {
            CsvDataBase.ReadWithAbsolutePath<T>(absolutePath);
        }

        /// <summary>
        /// Read from a standard CSV string.
        /// </summary>
        /// <param name="filePath"> Use for logging broken file. It can be empty. </param>
        /// <param name="content"> The complete CSV string. </param>
        public static void ReadFromString<T>(string filePath, string content) where T : CsvDataBase<T>, new()
        {
            CsvDataBase.ReadFromString<T>(filePath, content);
        }
    }
}
