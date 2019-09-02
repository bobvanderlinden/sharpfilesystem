using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpFileSystem.Collections;

namespace SharpFileSystem.FileSystems
{
    public class FileSystemMounter : IFileSystem
    {
        public FileSystemMounter(IEnumerable<KeyValuePair<FilePath, IFileSystem>> mounts)
        {
            Mounts = new SortedList<FilePath, IFileSystem>(new InverseComparer<FilePath>(Comparer<FilePath>.Default));
            foreach (var mount in mounts)
                Mounts.Add(mount);
        }

        public FileSystemMounter(params KeyValuePair<FilePath, IFileSystem>[] mounts)
            : this((IEnumerable<KeyValuePair<FilePath, IFileSystem>>)mounts)
        {
        }

        public ICollection<KeyValuePair<FilePath, IFileSystem>> Mounts { get; private set; }

        public void CreateDirectory(FilePath path)
        {
            var pair = Get(path);
            pair.Value.CreateDirectory(path.RemoveParent(pair.Key));
        }

        public Stream CreateFile(FilePath path)
        {
            var pair = Get(path);
            return pair.Value.CreateFile(path.RemoveParent(pair.Key));
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(FilePath path)
        {
            var pair = Get(path);
            pair.Value.Delete(path.RemoveParent(pair.Key));
        }

        public void Dispose()
        {
            foreach (var mount in Mounts.Select(p => p.Value))
                mount.Dispose();
        }

        public bool Exists(FilePath path)
        {
            var pair = Get(path);
            return pair.Value.Exists(path.RemoveParent(pair.Key));
        }

        public FilePath GetCurrentDirectory()
        {
            throw new System.NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            var pair = Get(path);
            var entities = pair.Value.GetEntities(path.IsRoot ? path : path.RemoveParent(pair.Key));
            return new EnumerableCollection<FilePath>(entities.Select(p => pair.Key.AppendPath(p)), entities.Count);
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            var pair = Get(path);
            return pair.Value.OpenFile(path.RemoveParent(pair.Key), access);
        }

        public string ReadAllText(FilePath path)
        {
            throw new System.NotImplementedException();
        }

        protected KeyValuePair<FilePath, IFileSystem> Get(FilePath path)
        {
            return Mounts.First(pair => pair.Key == path || pair.Key.IsParentOf(path));
        }
    }
}
