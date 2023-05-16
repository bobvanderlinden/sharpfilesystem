using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpFileSystem.FileSystems
{
    public abstract class AbstractFileSystem : IFileSystem
    {
        protected FileSystemPath Root { get; set; }

        public abstract ICollection<FileSystemPath> GetEntities(FileSystemPath path);
        public abstract bool Exists(FileSystemPath path);
        public abstract Stream CreateFile(FileSystemPath path);
        public abstract Stream OpenFile(FileSystemPath path, FileAccess access);
        public abstract void CreateDirectory(FileSystemPath path);
        public abstract void Delete(FileSystemPath path);


        public virtual void ChRoot(FileSystemPath newRoot)
        {
            Root = newRoot;
        }

        public virtual ICollection<FileSystemPath> GetFiles(FileSystemPath path)
        {
            if (!path.IsDirectory)
            {
                throw new InvalidOperationException("Path must be a directory.");
            }
            var entities = GetEntities(path);
            var files = entities.Where(x => x.IsFile && x.ParentPath == path.Path).ToList();
            return files;
        }


        public virtual ICollection<FileSystemPath> GetDirectories(FileSystemPath path)
        {
            var directories = new List<FileSystemPath>();
            var entities = GetEntities(path);
            foreach (var entity in entities)
            {

                    var parents = entity.ParentPath.GetDirectorySegments();
                    if (parents.Any() || path != FileSystemPath.DirectorySeparator.ToString())
                    {
                        directories.Add(FileSystemPath.DirectorySeparator+parents.First());
                    }
            }

            return directories.Distinct().ToList();
        }

        public abstract bool IsReadOnly { get; }

        public abstract void Dispose();
    }
}
