using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using SharpFileSystem.FileSystems;
using SharpFileSystem.IO;

namespace SharpFileSystem.SharpZipLib
{
    public class SharpZipLibFileSystem: IFileSystem
    {
        public ZipFile ZipFile { get; set; }

        public static SharpZipLibFileSystem Open(Stream s)
        {
            return new SharpZipLibFileSystem(new ZipFile(s));
        }

        public static SharpZipLibFileSystem Create(Stream s)
        {
            return new SharpZipLibFileSystem(ZipFile.Create(s));
        }

        private SharpZipLibFileSystem(ZipFile zipFile)
        {
            ZipFile = zipFile;
        }

        public void Dispose()
        {
            if (ZipFile.IsUpdating)
                ZipFile.CommitUpdate();
            ZipFile.Close();
        }

        protected FileSystemPath ToPath(ZipEntry entry)
        {
            return FileSystemPath.Parse(FileSystemPath.DirectorySeparator + entry.Name);
        }

        protected string ToEntryPath(FileSystemPath path)
        {
            // Remove heading '/' from path.
            return path.Path.TrimStart(FileSystemPath.DirectorySeparator);
        }

        protected ZipEntry ToEntry(FileSystemPath path)
        {
            return ZipFile.GetEntry(ToEntryPath(path));
        }

        protected IEnumerable<ZipEntry> GetZipEntries()
        {
            return ZipFile.Cast<ZipEntry>();
        }

        public ICollection<FileSystemPath> GetEntities(FileSystemPath path)
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

        public bool Exists(FileSystemPath path)
        {
            if (path.IsFile)
                return ToEntry(path) != null;
            return GetZipEntries()
                .Select(ToPath)
                .Any(entryPath => entryPath.IsChildOf(path));
        }

        public Stream CreateFile(FileSystemPath path)
        {
            return CreateFile(path,null);
        }
        public Stream CreateFile(FileSystemPath path, byte[] data)
        {
            BeginUpdate();
            var entry = new MemoryZipEntry();
            ZipFile.Add(entry, ToEntryPath(path));
            var s = entry.GetSource();

            if (data!=null) s.Write(data);

            EndUpdate();
            return s;
        }
        private void BeginUpdate()
        {
            if (!ZipFile.IsUpdating)
                ZipFile.BeginUpdate();
        }
        private void EndUpdate()
        {
            if (ZipFile.IsUpdating)
                ZipFile.CommitUpdate();
        }

        public Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            if (access != FileAccess.Read)
                throw new NotSupportedException();
            return ZipFile.GetInputStream(ToEntry(path));
        }

        public void CreateDirectory(FileSystemPath path)
        {
            BeginUpdate();
            ZipFile.AddDirectory(ToEntryPath(path));
            EndUpdate();
        }

        public void Delete(FileSystemPath path)
        {
            BeginUpdate();
            ZipFile.Delete(ToEntryPath(path));
            EndUpdate();
        }

        public class MemoryZipEntry: MemoryFileSystem.MemoryFile, IStaticDataSource
        {
            public Stream GetSource()
            {
                return new MemoryFileSystem.MemoryFileStream(this);
            }
        }
    }
}
