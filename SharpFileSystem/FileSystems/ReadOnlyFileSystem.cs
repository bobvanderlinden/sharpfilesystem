using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class ReadOnlyFileSystem : IFileSystem
    {
        public ReadOnlyFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public IFileSystem FileSystem { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public Stream CreateFile(FilePath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public void Delete(FilePath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public void Dispose()
        {
            FileSystem.Dispose();
        }

        public bool Exists(FilePath path)
        {
            return FileSystem.Exists(path);
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            return FileSystem.GetEntities(path);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (access != FileAccess.Read)
                throw new InvalidOperationException("This is a read-only filesystem.");
            return FileSystem.OpenFile(path, access);
        }
    }
}
