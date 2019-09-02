using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using SharpFileSystem.FileSystems;

namespace SharpFileSystem.SharpZipLib
{
    public class SharpZipLibFileSystem : IFileSystem
    {
        private SharpZipLibFileSystem(ZipFile zipFile)
        {
            ZipFile = zipFile;
        }

        public ZipFile ZipFile { get; set; }

        public static SharpZipLibFileSystem Create(Stream s)
        {
            return new SharpZipLibFileSystem(ZipFile.Create(s));
        }

        public static SharpZipLibFileSystem Open(Stream s)
        {
            return new SharpZipLibFileSystem(new ZipFile(s));
        }

        public void CreateDirectory(FilePath path)
        {
            ZipFile.AddDirectory(ToEntryPath(path));
        }

        public Stream CreateFile(FilePath path)
        {
            var entry = new MemoryZipEntry();
            ZipFile.Add(entry, ToEntryPath(path));
            return entry.GetSource();
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            throw new NotImplementedException();
        }

        public void Delete(FilePath path)
        {
            ZipFile.Delete(ToEntryPath(path));
        }

        public void Dispose()
        {
            if (ZipFile.IsUpdating)
                ZipFile.CommitUpdate();
            ZipFile.Close();
        }

        public bool Exists(FilePath path)
        {
            if (path.IsFile)
                return ToEntry(path) != null;
            return GetZipEntries()
                .Select(ToPath)
                .Any(entryPath => entryPath.IsChildOf(path));
        }

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            return GetZipEntries()
                .Select(ToPath)
                .Where(entryPath => path.IsParentOf(entryPath))
                .Select(entryPath => entryPath.ParentPath == path
                    ? entryPath
                    : path.AppendDirectory(entryPath.RemoveParent(path).GetDirectorySegments()[0])
                    )
                .Distinct()
                .ToList();
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (access != FileAccess.Read)
                throw new NotSupportedException();
            return ZipFile.GetInputStream(ToEntry(path));
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }

        protected IEnumerable<ZipEntry> GetZipEntries()
        {
            return ZipFile.Cast<ZipEntry>();
        }

        protected ZipEntry ToEntry(FilePath path)
        {
            return ZipFile.GetEntry(ToEntryPath(path));
        }

        protected string ToEntryPath(FilePath path)
        {
            // Remove heading '/' from path.
            return path.Path.TrimStart(FilePath.DirectorySeparator);
        }

        protected FilePath ToPath(ZipEntry entry)
        {
            return FilePath.Parse(FilePath.DirectorySeparator + entry.Name);
        }

        public class MemoryZipEntry : MemoryFileSystem.MemoryFile, IStaticDataSource
        {
            public Stream GetSource()
            {
                return new MemoryFileSystem.MemoryFileStream(this);
            }
        }
    }
}
