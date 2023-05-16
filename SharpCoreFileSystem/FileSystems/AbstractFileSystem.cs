using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpFileSystem.FileSystems
{
    public abstract class AbstractFileSystem : IFileSystem
    {
        protected FileSystemPath Root { get; set; }

        public abstract ICollection<FileSystemPath> GetEntities(FileSystemPath path);
        public abstract bool Exists(FileSystemPath path);
        public abstract Stream CreateFile(FileSystemPath path);
        public abstract Stream OpenFile(FileSystemPath path, FileAccess access);
        public abstract void CreateDirectory(FileSystemPath path);
        public abstract void Delete(FileSystemPath path);

        public ICollection<FileSystemPath> GetFiles(FileSystemPath path)
        {
            var files = GetEntities(path).Where(x => x.IsFile).ToList();
            return files;
        }



        public virtual void ChRoot(FileSystemPath newRoot)
        {
            Root = newRoot;
        }

        public ICollection<FileSystemPath> GetDirectories(FileSystemPath path)
        {
            var directories = GetEntities(path).Where(x => x.IsDirectory).ToList();
            return directories;
        }

        public abstract bool IsReadOnly { get; }

        public abstract void Dispose();
    }
}
