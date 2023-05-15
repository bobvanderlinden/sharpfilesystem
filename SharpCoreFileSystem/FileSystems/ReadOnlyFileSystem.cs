using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class ReadOnlyFileSystem: AbstractFileSystem
    {
        public new ICollection<FileSystemPath> GetFiles(FileSystemPath path) => FileSystem.GetFiles(path);

        public override bool IsReadOnly => true;

        public IFileSystem FileSystem { get; private set; }

        public ReadOnlyFileSystem(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public override void Dispose()
        {
            FileSystem.Dispose();
        }

        public override ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            return FileSystem.GetEntities(path);
        }

        public override bool Exists(FileSystemPath path)
        {
            return FileSystem.Exists(path);
        }

        public override Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            if (access != FileAccess.Read)
                throw new InvalidOperationException("This is a read-only filesystem.");
            return FileSystem.OpenFile(path, access);
        }

        public override Stream CreateFile(FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public override void CreateDirectory(FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public override void Delete(FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public new ICollection<FileSystemPath> GetDirectories(FileSystemPath path) => FileSystem.GetDirectories(path);

    }
}
