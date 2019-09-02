using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class FileSystemWrapper : IFileSystem
    {
        public FileSystemWrapper(IFileSystem parent)
        {
            Parent = parent;
        }

        public IFileSystem Parent { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            Parent.CreateDirectory(path);
        }

        public Stream CreateFile(FilePath path)
        {
            return Parent.CreateFile(path);
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            throw new NotImplementedException();
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

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            return Parent.GetEntities(path);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            return Parent.OpenFile(path, access);
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }
    }
}
