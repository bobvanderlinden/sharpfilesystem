using System;
using System.Collections.Generic;
using System.Linq;
using SevenZip;
using System.IO;
using System.Threading;
using SharpFileSystem;
using SharpFileSystem.IO;
using Directory = SharpFileSystem.Directory;
using File = SharpFileSystem.File;

namespace SharpFileSystem.SevenZip
{
    public class SevenZipFileSystem : IFileSystem
    {
        private ICollection<FilePath> _entities = new List<FilePath>();
        private SevenZipExtractor _extractor;
        public SevenZipFileSystem(Stream stream)
            : this(new SevenZipExtractor(stream))
        {
        }

        public SevenZipFileSystem(string physicalPath)
            : this(new SevenZipExtractor(physicalPath))
        {
        }

        private SevenZipFileSystem(SevenZipExtractor extractor)
        {
            _extractor = extractor;
            foreach (var file in _extractor.ArchiveFileData)
                AddEntity(GetVirtualFilePath(file));
        }

        public void AddEntity(FilePath path)
        {
            if (!_entities.Contains(path))
                _entities.Add(path);
            if (!path.IsRoot)
                AddEntity(path.ParentPath);
        }

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
            _extractor.Dispose();
        }

        public bool Exists(FilePath path)
        {
            return _entities.Contains(path);
        }

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is not a directory.", "path");
            return _entities.Where(p => !p.IsRoot && p.ParentPath.Equals(path)).ToArray();
        }

        public string GetSevenZipPath(FilePath path)
        {
            return path.ToString().Remove(0, 1);
        }

        public FilePath GetVirtualFilePath(ArchiveFileInfo archiveFile)
        {
            string path = FilePath.DirectorySeparator + archiveFile.FileName.Replace(Path.DirectorySeparatorChar, FilePath.DirectorySeparator);
            if (archiveFile.IsDirectory && path[path.Length - 1] != FilePath.DirectorySeparator)
                path += FilePath.DirectorySeparator;
            return FilePath.Parse(path);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (access == FileAccess.Write)
                throw new NotSupportedException();

            Stream s = new ProducerConsumerStream();
            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 _extractor.ExtractFile(GetSevenZipPath(path), s);
                                                 s.Close();
                                             });
            return s;
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }
    }
}
