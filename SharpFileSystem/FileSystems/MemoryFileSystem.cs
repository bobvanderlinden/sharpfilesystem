using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpFileSystem.FileSystems
{
    public class MemoryFileSystem : IFileSystem
    {
        public MemoryFileSystem()
        {
            Directories.Add(FilePath.Root, new HashSet<FilePath>());
        }

        private IDictionary<FilePath, ISet<FilePath>> Directories { get; }
                    = new Dictionary<FilePath, ISet<FilePath>>();

        private IDictionary<FilePath, MemoryFile> Files { get; } =
                            new Dictionary<FilePath, MemoryFile>();

        public void CreateDirectory(FilePath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is no directory.", "path");
            ISet<FilePath> subentities;
            if (Directories.ContainsKey(path))
                throw new ArgumentException("The specified directory-path already exists.", "path");
            if (!Directories.TryGetValue(path.ParentPath, out subentities))
                throw new DirectoryNotFoundException();
            subentities.Add(path);
            Directories[path] = new HashSet<FilePath>();
        }

        public Stream CreateFile(FilePath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is no file.", "path");
            if (!Directories.ContainsKey(path.ParentPath))
                throw new DirectoryNotFoundException();
            Directories[path.ParentPath].Add(path);
            return new MemoryFileStream(Files[path] = new MemoryFile());
        }

        public void CreateTextFile(FilePath path, string contents)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is no file.", nameof(path));
            if (!Directories.ContainsKey(path.ParentPath))
                throw new DirectoryNotFoundException();
            Directories[path.ParentPath].Add(path);
            var file = Files[path] = new MemoryFile();
            var stream = new MemoryFileStream(file);
            var bytes = new UTF8Encoding().GetBytes(contents);
            stream.Write(bytes, 0, contents.Length);
            stream.Close();
        }

        public void Delete(FilePath path)
        {
            if (path.IsRoot)
                throw new ArgumentException("The root cannot be deleted.");

            var removed = path.IsDirectory
                ? Directories.Remove(path)
                : Files.Remove(path);
            if (!removed)
                throw new ArgumentException("The specified path does not exist.");
            var parent = Directories[path.ParentPath];
            parent.Remove(path);
        }

        public void Dispose()
        {
            // ignored
        }

        public bool Exists(FilePath path)
        {
            return path.IsDirectory
                ? Directories.ContainsKey(path)
                : Files.ContainsKey(path);
        }

        public FilePath GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public ICollection<FilePath> GetEntities(FilePath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is no directory.", "path");
            ISet<FilePath> subentities;
            if (!Directories.TryGetValue(path, out subentities))
                throw new DirectoryNotFoundException();
            return subentities;
        }

        public Stream OpenFile(FilePath path, FileAccess access)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is no file.", "path");
            MemoryFile file;
            if (!Files.TryGetValue(path, out file))
                throw new FileNotFoundException();
            return new MemoryFileStream(file);
        }

        public string ReadAllText(FilePath path)
        {
            throw new NotImplementedException();
        }

        public class MemoryFile
        {
            public MemoryFile()
                : this(new byte[0])
            {
            }

            public MemoryFile(byte[] content)
            {
                Content = content;
            }

            public byte[] Content { get; set; }
        }

        public class MemoryFileStream : Stream
        {
            private readonly MemoryFile _file;

            public MemoryFileStream(MemoryFile file)
            {
                _file = file;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public byte[] Content
            {
                get { return _file.Content; }
                set { _file.Content = value; }
            }

            public override long Length
            {
                get { return _file.Content.Length; }
            }

            public override long Position { get; set; }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int mincount = Math.Min(count, Math.Abs((int)(Length - Position)));
                Buffer.BlockCopy(Content, (int)Position, buffer, offset, mincount);
                Position += mincount;
                return mincount;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                if (origin == SeekOrigin.Begin)
                    return Position = offset;
                if (origin == SeekOrigin.Current)
                    return Position += offset;
                return Position = Length - offset;
            }

            public override void SetLength(long value)
            {
                int newLength = (int)value;
                byte[] newContent = new byte[newLength];
                Buffer.BlockCopy(Content, 0, newContent, 0, Math.Min(newLength, (int)Length));
                Content = newContent;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (Length - Position < count)
                    SetLength(Position + count);
                Buffer.BlockCopy(buffer, offset, Content, (int)Position, count);
                Position += count;
            }
        }
    }
}
