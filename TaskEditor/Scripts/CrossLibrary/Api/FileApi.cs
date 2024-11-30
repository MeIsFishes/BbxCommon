using System;
using System.Collections.Generic;
using System.IO;

namespace BbxCommon
{
    public static class FileApi
    {
        public static void CreateAbsoluteDirectory(string path)
        {
            path = path.Replace("\\", "/");
            if (path.EndsWith("/") == false)
            {
                int index = path.LastIndexOf("/");
                path = path.Remove(index + 1);
            };
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Create file with the given path, and ensure that all directories in path will be created.
        /// </summary>
        public static void CreateAbsolutePathFile(string path)
        {
            CreateAbsoluteDirectory(path);
            var fileStream = File.Create(path);
            fileStream.Close();
        }

        /// <summary>
        /// AddExtensionIfNot("C:/MyFile/game", ".exe") returns "C:/MyFile/game.exe".
        /// </summary>
        public static string AddExtensionIfNot(string path, string extension)
        {
            if (extension.StartsWith(".") == false)
                extension = "." + extension;
            if (path.EndsWith(extension) == false)
                path += extension; 
            return path;
        }

        /// <summary>
        /// "C:/MyFile/game" returns null. "C:/MyFile/game.exe" returns ".exe".
        /// </summary>
        /// <param name="withDot"> If true, return ".exe", else return "exe". </param>
        public static string GetExtensionIfHas(string path, bool withDot = true)
        {
            var index = path.LastIndexOf('.');
            var indexOfSeparator = Math.Max(path.LastIndexOf("\\"), path.LastIndexOf("/"));
            if (indexOfSeparator > index)
                return null;
            if (index == path.Length - 1)
                return null;
            if (withDot)
                return path.Substring(index);
            else
                return path.Substring(index + 1);
        }

        /// <summary>
        /// "C:/MyFile/game.exe" returns "C:/MyFile/game".
        /// </summary>
        public static string RemoveExtensionIfHas(string path)
        {
            var index = path.LastIndexOf('.');
            var indexOfSeparator = Math.Max(path.LastIndexOf("\\"), path.LastIndexOf("/"));
            if (indexOfSeparator > index)
                return path;
            return path.Remove(index);
        }

        /// <summary>
        /// "C:/MyFile/Game/" returns "Game". "C:/MyFile/game.exe" returns "game.exe".
        /// </summary>
        public static string GetLastDirectoryOrFileOfPath(string path)
        {
            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1);
            var index1 = path.LastIndexOf('/');
            var index2 = path.LastIndexOf("\\");
            if (index1 >= 0 || index2 >= 0)
            {
                if (index1 > index2)
                    return path.Substring(index1);
                else
                    return path.Substring(index2 + 1);
            }
            return path;
        }

        public static List<string> SearchAllFilesInFolder(string path)
        {
            var res = new List<string>();
            if (Directory.Exists(path) == false)
            {
                DebugApi.LogError("You require files in " + path + ", but the directory doesn't exist!");
                return res;
            }
            var curDirectories = new List<string>();
            var newDirectories = new List<string>();
            curDirectories.Add(path);
            // recursively search all files from the directories
            while (curDirectories.Count > 0)
            {
                foreach (var dir in curDirectories)
                {
                    var foundDirectories = Directory.GetDirectories(dir);
                    for (int j = 0; j < foundDirectories.Length; j++)
                    {
                        newDirectories.Add(foundDirectories[j]);
                    }
                    var foundFiles = Directory.GetFiles(dir);
                    for (int j = 0; j < foundFiles.Length; j++)
                    {
                        var file = foundFiles[j];
                        res.Add(file);
                    }
                }
                curDirectories.Clear();
                foreach (var dir in newDirectories)
                {
                    curDirectories.Add(dir);
                }
                newDirectories.Clear();
            }
            return res;
        }

        public static string GetAbsolutePath(string path)
        {
            return Path.GetFullPath(path);
        }
    }
}