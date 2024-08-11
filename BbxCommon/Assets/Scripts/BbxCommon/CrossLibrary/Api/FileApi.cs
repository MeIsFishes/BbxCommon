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

        public static void CreateAbsolutePathFile(string path)
        {
            CreateAbsoluteDirectory(path);
            var fileStream = File.Create(path);
            fileStream.Close();
        }
    }
}
