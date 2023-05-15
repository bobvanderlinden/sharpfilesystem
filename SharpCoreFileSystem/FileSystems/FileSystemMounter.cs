using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpFileSystem.Collections;

namespace SharpFileSystem.FileSystems
{

    public class MountPoint
    {
        public IFileSystem FS {get; set;}
        public FileSystemPath Path {get; set;}

        public MountPoint(FileSystemPath path, IFileSystem fs)
        {
            Path = path;
            FS = fs;
        }
    }

    public class FileSystemMounter : AbstractFileSystem
    {

        public override bool IsReadOnly => Mounts.All(x => x.Value.IsReadOnly);

        public ICollection<KeyValuePair<FileSystemPath, IFileSystem>> Mounts { get; private set; }

        public FileSystemMounter(IEnumerable<KeyValuePair<FileSystemPath, IFileSystem>> mounts)
        {
            Mounts = new SortedList<FileSystemPath, IFileSystem>(new InverseComparer<FileSystemPath>(Comparer<FileSystemPath>.Default));
            foreach(var mount in mounts)
                Mounts.Add(mount);
        }

        public FileSystemMounter(params (FileSystemPath path, IFileSystem fs)[] mounts) : this (mounts.Select(x => new KeyValuePair<FileSystemPath, IFileSystem>(x.path,x.fs)))
        {
        }

        public FileSystemMounter(params MountPoint[] mounts) : this (mounts.Select(x => new KeyValuePair<FileSystemPath, IFileSystem>(x.Path,x.FS)))
        {
        }

        public FileSystemMounter(params KeyValuePair<FileSystemPath, IFileSystem>[] mounts)
            : this((IEnumerable<KeyValuePair<FileSystemPath, IFileSystem>>)mounts)
        {
        }

        protected KeyValuePair<FileSystemPath, IFileSystem> Get(FileSystemPath path)
        {
            return Mounts.First(pair => pair.Key == path || pair.Key.IsParentOf(path));
        }

        public override void Dispose()
        {
            foreach (var mount in Mounts.Select(p => p.Value))
                mount.Dispose();
        }

        public override ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            var pair = Get(path);
            var entities = pair.Value.GetEntities(path.IsRoot ? path : path.RemoveParent(pair.Key));
            return new EnumerableCollection<FileSystemPath>(entities.Select(p => pair.Key.AppendPath(p)), entities.Count);
        }

        public override bool Exists(FileSystemPath path)
        {
            var pair = Get(path);
            return pair.Value.Exists(path.RemoveParent(pair.Key));
        }

        public override Stream CreateFile(FileSystemPath path)
        {
            var pair = Get(path);
            return pair.Value.CreateFile(path.RemoveParent(pair.Key));
        }

        public override Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            var pair = Get(path);
            return pair.Value.OpenFile(path.RemoveParent(pair.Key), access);
        }

        public override void CreateDirectory(FileSystemPath path)
        {
            var pair = Get(path);
            pair.Value.CreateDirectory(path.RemoveParent(pair.Key));
        }

        public override void Delete(FileSystemPath path)
        {
            var pair = Get(path);
            pair.Value.Delete(path.RemoveParent(pair.Key));
        }
    }
}
