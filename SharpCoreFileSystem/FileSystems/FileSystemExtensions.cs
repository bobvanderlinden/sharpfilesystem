using System.IO;

namespace SharpFileSystem.FileSystems
{
    public static class FileSystemExtensions
    {
        public static string ReadAllText(this IFileSystem fileSystem, FileSystemPath path)
        {
            string content = "";
            if (fileSystem.Exists(path))
            {
                using (var stream = fileSystem.OpenFile(path, FileAccess.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }

            return content;
        }
    }
}
