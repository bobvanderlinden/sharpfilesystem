using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class MergedFileSystem : IFileSystem
    {
        public MergedFileSystem(IEnumerable<IFileSystem> fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public MergedFileSystem(params IFileSystem[] fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public IEnumerable<IFileSystem> FileSystems { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            if (Exists(path))
                throw new ArgumentException("The specified directory already exists.");
            var fs = GetFirst(path.ParentPath);
            if (fs == null)
                throw new ArgumentException("The directory-parent does not exist.");
            fs.CreateDirectory(path);
        }

        public Stream CreateFile(FilePath path)
        {
            var fs = GetFirst(path) ?? FileSystems.First();
            return fs.CreateFile(path);
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            throw new NotImplementedException();
        }

        public void Delete(FilePath path)
        {
            foreach (var fs in FileSystems.Where(fs => fs.Exists(path)))
                fs.Delete(path);
        }

        public void Dispose()
        {
            foreach (var fs in FileSystems)
                fs.Dispose();
        }

        public bool Exists(FilePath path)
        {
            return FileSystems.Any(fs => fs.Exists(path));
        }

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            var entities = new SortedList<FilePath, FilePath>();
            foreach (var fs in FileSystems.Where(fs => fs.Exists(path)))
            {
                foreach (var entity in fs.GetEntities(path))
                    if (!entities.ContainsKey(entity))
                        entities.Add(entity, entity);
            }
            return entities.Values;
        }

        public IFileSystem GetFirst(FilePath path)
        {
            return FileSystems.FirstOrDefault(fs => fs.Exists(path));
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            var fs = GetFirst(path);
            if (fs == null)
                throw new FileNotFoundException();
            return fs.OpenFile(path, access);
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }
    }
}
