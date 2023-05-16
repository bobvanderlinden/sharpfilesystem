using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class MergedFileSystem: AbstractFileSystem
    {
        public override bool IsReadOnly => FileSystems.All(x => x.IsReadOnly);

        public IEnumerable<IFileSystem> FileSystems { get; private set; }
        public MergedFileSystem(IEnumerable<IFileSystem> fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public MergedFileSystem(params IFileSystem[] fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public override void Dispose()
        {
            foreach(var fs in FileSystems)
                fs.Dispose();
        }

        public override ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            var entities = new SortedList<FileSystemPath, FileSystemPath>();
            foreach (var fs in FileSystems.Where(fs => fs.Exists(path)))
            {
                foreach(var entity in fs.GetEntities(path))
                    if (!entities.ContainsKey(entity))
                        entities.Add(entity, entity);
            }
            return entities.Values;
        }

        public override bool Exists(FileSystemPath path)
        {
            return FileSystems.Any(fs => fs.Exists(path));
        }

        public IFileSystem GetFirst(FileSystemPath path)
        {
            return FileSystems.FirstOrDefault(fs => fs.Exists(path));
        }

        public IFileSystem GetFirstRW(FileSystemPath path)
        {
            return FileSystems.FirstOrDefault(fs => !fs.IsReadOnly && fs.Exists(path));
        }

        public override Stream CreateFile(FileSystemPath path)
        {
            var fs = GetFirstRW(path) ?? FileSystems.First();
            return fs.CreateFile(path);
        }

        public override Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            var fs = GetFirst(path);
            if (fs == null)
                throw new FileNotFoundException();
            return fs.OpenFile(path, access);
        }

        public override void CreateDirectory(FileSystemPath path)
        {
            if (Exists(path))
                throw new ArgumentException("The specified directory already exists.");
            var fs = GetFirstRW(path.ParentPath);
            if (fs == null)
                throw new ArgumentException("The directory-parent does not exist.");
            fs.CreateDirectory(path);
        }

        public override void Delete(FileSystemPath path)
        {
            foreach(var fs in FileSystems.Where(fs => fs.Exists(path)))
                fs.Delete(path);
        }

        public override void ChRoot(FileSystemPath root)
        {
            Root = root;
            foreach (var fileSystem in FileSystems)
            {
                fileSystem.ChRoot(root);
            }
        }
    }
}
