using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpFileSystem.Collections;

namespace SharpFileSystem.FileSystems
{
    public class SubFileSystem : IFileSystem
    {
        public SubFileSystem(IFileSystem fileSystem, FilePath root)
        {
            FileSystem = fileSystem;
            Root = root;
        }

        public IFileSystem FileSystem { get; private set; }

        public FilePath Root { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            FileSystem.CreateDirectory(AppendRoot(path));
        }

        public Stream CreateFile(FilePath path)
        {
            return FileSystem.CreateFile(AppendRoot(path));
        }

        public void Delete(FilePath path)
        {
            FileSystem.Delete(AppendRoot(path));
        }

        public void Dispose()
        {
            FileSystem.Dispose();
        }

        public bool Exists(FilePath path)
        {
            return FileSystem.Exists(AppendRoot(path));
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            var paths = FileSystem.GetEntities(AppendRoot(path));
            return new EnumerableCollection<FilePath>(paths.Select(p => RemoveRoot(p)), paths.Count);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            return FileSystem.OpenFile(AppendRoot(path), access);
        }

        protected FilePath AppendRoot(FilePath path)
        {
            return Root.AppendPath(path);
        }

        protected FilePath RemoveRoot(FilePath path)
        {
            return path.RemoveParent(Root);
        }
    }
}
