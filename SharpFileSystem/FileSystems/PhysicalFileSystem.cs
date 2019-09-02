using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpFileSystem.Collections;

namespace SharpFileSystem.FileSystems
{
    public class PhysicalFileSystem : IFileSystem
    {
        public PhysicalFileSystem(string physicalRoot)
        {
            if (!Path.IsPathRooted(physicalRoot))
                physicalRoot = Path.GetFullPath(physicalRoot);
            if (physicalRoot[physicalRoot.Length - 1] != Path.DirectorySeparatorChar)
                physicalRoot = physicalRoot + Path.DirectorySeparatorChar;
            PhysicalRoot = physicalRoot;
        }

        public string PhysicalRoot { get; private set; }

        public string GetPhysicalPath(FilePath path)
        {
            return Path.Combine(PhysicalRoot, path.ToString().Remove(0, 1).Replace(FilePath.DirectorySeparator, Path.DirectorySeparatorChar));
        }

        public FilePath GetVirtualDirectoryPath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            string virtualPath = FilePath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, FilePath.DirectorySeparator);
            if (virtualPath[virtualPath.Length - 1] != FilePath.DirectorySeparator)
                virtualPath += FilePath.DirectorySeparator;
            return FilePath.Parse(virtualPath);
        }

        public FilePath GetVirtualFilePath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            string virtualPath = FilePath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, FilePath.DirectorySeparator);
            return FilePath.Parse(virtualPath);
        }

        public void CreateDirectory(FilePath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is not a directory.", "path");
            System.IO.Directory.CreateDirectory(GetPhysicalPath(path));
        }

        public Stream CreateFile(FilePath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return System.IO.File.Create(GetPhysicalPath(path));
        }

        public void Delete(FilePath path)
        {
            if (path.IsFile)
                System.IO.File.Delete(GetPhysicalPath(path));
            else
                System.IO.Directory.Delete(GetPhysicalPath(path), true);
        }

        public void Dispose()
        {
        }

        public bool Exists(FilePath path)
        {
            return path.IsFile ? System.IO.File.Exists(GetPhysicalPath(path)) : System.IO.Directory.Exists(GetPhysicalPath(path));
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            string physicalPath = GetPhysicalPath(path);
            string[] directories = System.IO.Directory.GetDirectories(physicalPath);
            string[] files = System.IO.Directory.GetFiles(physicalPath);
            var virtualDirectories =
                directories.Select(p => GetVirtualDirectoryPath(p));
            var virtualFiles =
                files.Select(p => GetVirtualFilePath(p));
            return new EnumerableCollection<FilePath>(virtualDirectories.Concat(virtualFiles), directories.Length + files.Length);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return System.IO.File.Open(GetPhysicalPath(path), FileMode.Open, access);
        }
    }
}
