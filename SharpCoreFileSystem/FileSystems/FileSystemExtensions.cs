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

        public static void WriteAllText(this IFileSystem fileSystem, FileSystemPath path, string content)
        {
            if (!fileSystem.Exists(path))
            {
                fileSystem.CreateFile(path);
            }

            using (var stream = fileSystem.OpenFile(path, FileAccess.Write))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                }
            }
        }
    }
}
