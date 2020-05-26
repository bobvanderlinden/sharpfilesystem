using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SharpFileSystem.Resources
{

    public static class AssemblyExtensions
    {
        public static string GetShortName(this Assembly assembly)
        {
            return assembly.FullName.Split(new[] {','}).First();
        }
    }
    public class EmbeddedResourceFileSystem : IFileSystem
    {
        public Assembly Assembly { get; private set; }

        private string AssemblyName => Assembly.GetShortName();
        public EmbeddedResourceFileSystem(Assembly assembly)
        {
            Assembly = assembly;
        }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            if (!path.IsRoot)
                throw new DirectoryNotFoundException();
            return Assembly.GetManifestResourceNames().Select(name => FileSystemPath.Root.AppendFile(name.Replace(AssemblyName+".",""))).ToArray();
        }

        public bool Exists(FileSystemPath path)
        {
            return path.IsRoot || !path.IsDirectory && Assembly.GetManifestResourceNames().Contains($"{AssemblyName}.{path.Path.Substring(1).Replace("/",".")}");
        }

        public Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            if (access == FileAccess.Write)
                throw new NotSupportedException();
            // if (path.IsDirectory || path.ParentPath != FileSystemPath.Root)
            //     throw new FileNotFoundException();
            return Assembly.GetManifestResourceStream($"{AssemblyName}.{path.Path.Substring(1).Replace("/",".")}");
        }

        public Stream CreateFile(FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void CreateDirectory(FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void Delete(FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}
