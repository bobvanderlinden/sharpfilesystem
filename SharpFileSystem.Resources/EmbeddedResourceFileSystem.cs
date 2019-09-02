using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SharpFileSystem.Resources
{
    public class EmbeddedResourceFileSystem : IFileSystem
    {
        public EmbeddedResourceFileSystem(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Assembly Assembly { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            throw new NotSupportedException();
        }

        public Stream CreateFile(FilePath path)
        {
            throw new NotSupportedException();
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            throw new NotImplementedException();
        }

        public void Delete(FilePath path)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }

        public bool Exists(FilePath path)
        {
            return path.IsRoot || !path.IsDirectory && Assembly.GetManifestResourceNames().Contains(path.EntityName);
        }

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            if (!path.IsRoot)
                throw new DirectoryNotFoundException();
            return Assembly.GetManifestResourceNames().Select(name => FilePath.Root.AppendFile(name)).ToArray();
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (access == FileAccess.Write)
                throw new NotSupportedException();
            if (path.IsDirectory || path.ParentPath != FilePath.Root)
                throw new FileNotFoundException();
            return Assembly.GetManifestResourceStream(path.EntityName);
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }
    }
}
