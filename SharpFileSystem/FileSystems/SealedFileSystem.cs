using System.Collections.Generic;
using System.IO;

namespace SharpFileSystem.FileSystems
{
    public class SealedFileSystem : IFileSystem
    {
        public SealedFileSystem(IFileSystem parent)
        {
            Parent = parent;
        }

        private IFileSystem Parent { get; set; }

        public void CreateDirectory(FilePath path)
        {
            Parent.CreateDirectory(path);
        }

        public Stream CreateFile(FilePath path)
        {
            return Parent.CreateFile(path);
        }

        public void Delete(FilePath path)
        {
            Parent.Delete(path);
        }

        public void Dispose()
        {
            Parent.Dispose();
        }

        public bool Exists(FilePath path)
        {
            return Parent.Exists(path);
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            return Parent.GetEntities(path);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            return Parent.OpenFile(path, access);
        }
    }
}
