using System;

namespace SharpFileSystem
{
    public class FileSystemEntity : IEquatable<FileSystemEntity>
    {
        public FileSystemEntity(IFileSystem fileSystem, FilePath path)
        {
            FileSystem = fileSystem;
            Path = path;
        }

        public IFileSystem FileSystem { get; private set; }

        public string Name { get { return Path.EntityName; } }

        public FilePath Path { get; private set; }

        public static FileSystemEntity Create(IFileSystem fileSystem, FilePath path)
        {
            if (path.IsFile)
                return new File(fileSystem, path);
            else
                return new Directory(fileSystem, path);
        }

        public override bool Equals(object obj)
        {
            var other = obj as FileSystemEntity;
            return (other != null) && ((IEquatable<FileSystemEntity>)this).Equals(other);
        }

        bool IEquatable<FileSystemEntity>.Equals(FileSystemEntity other)
        {
            return FileSystem.Equals(other.FileSystem) && Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            return FileSystem.GetHashCode() ^ Path.GetHashCode();
        }
    }
}
