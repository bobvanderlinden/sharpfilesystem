using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SharpFileSystem
{
    public struct FilePath : IEquatable<FilePath>, IComparable<FilePath>
    {
        public static readonly char DirectorySeparator = '/';
        private readonly string _path;
        static FilePath()
        {
            Root = new FilePath(DirectorySeparator.ToString());
        }

        private FilePath(string path)
        {
            _path = path;
        }

        public static FilePath Root { get; private set; }

        public string EntityName
        {
            get
            {
                var name = Path;
                if (IsRoot)
                    return null;
                var endOfName = name.Length;
                if (IsDirectory)
                    endOfName--;
                var startOfName = name.LastIndexOf(DirectorySeparator, endOfName - 1, endOfName) + 1;
                return name.Substring(startOfName, endOfName - startOfName);
            }
        }

        public bool IsDirectory => Path[Path.Length - 1] == DirectorySeparator;

        public bool IsFile => !IsDirectory;

        public bool IsRoot => Path.Length == 1;

        public FilePath ParentPath
        {
            get
            {
                var parentPath = Path;
                if (IsRoot)
                    throw new InvalidOperationException("There is no parent of root.");
                var lookaheadCount = parentPath.Length;
                if (IsDirectory)
                    lookaheadCount--;
                var index = parentPath.LastIndexOf(DirectorySeparator, lookaheadCount - 1, lookaheadCount);
                Debug.Assert(index >= 0);
                parentPath = parentPath.Remove(index + 1);
                return new FilePath(parentPath);
            }
        }

        public string Path => _path ?? "/";

        public static bool IsRooted(string s)
        {
            if (s.Length == 0)
                return false;
            return s[0] == DirectorySeparator;
        }

        public static bool operator !=(FilePath pathA, FilePath pathB)
        {
            return !(pathA == pathB);
        }

        public static bool operator ==(FilePath pathA, FilePath pathB)
        {
            return pathA.Equals(pathB);
        }

        public static FilePath Parse(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (!IsRooted(s))
                throw new ParseException(s, "Path is not rooted");
            if (s.Contains(string.Concat(DirectorySeparator, DirectorySeparator)))
                throw new ParseException(s, "Path contains double directory-separators.");
            return new FilePath(s);
        }

        [Pure]
        public FilePath AppendDirectory(string directoryName)
        {
            if (directoryName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("The specified name includes directory-separator(s).", nameof(directoryName));
            if (!IsDirectory)
                throw new InvalidOperationException("The specified FileSystemPath is not a directory.");
            return new FilePath(Path + directoryName + DirectorySeparator);
        }

        [Pure]
        public FilePath AppendFile(string fileName)
        {
            if (fileName.Contains(DirectorySeparator.ToString()))
                throw new ArgumentException("The specified name includes directory-separator(s).", nameof(fileName));
            if (!IsDirectory)
                throw new InvalidOperationException("The specified FileSystemPath is not a directory.");
            return new FilePath(Path + fileName);
        }

        public FilePath AppendPath(string relativePath)
        {
            if (IsRooted(relativePath))
                throw new ArgumentException("The specified path should be relative.", nameof(relativePath));
            if (!IsDirectory)
                throw new InvalidOperationException("This FileSystemPath is not a directory.");
            return new FilePath(Path + relativePath);
        }

        [Pure]
        public FilePath AppendPath(FilePath path)
        {
            if (!IsDirectory)
                throw new InvalidOperationException("This FileSystemPath is not a directory.");
            return new FilePath(Path + path.Path.Substring(1));
        }

        [Pure]
        public FilePath ChangeExtension(string extension)
        {
            if (!IsFile)
                throw new ArgumentException("The specified FileSystemPath is not a file.");
            var name = EntityName;
            var extensionIndex = name.LastIndexOf('.');
            return extensionIndex >= 0
                ? ParentPath.AppendFile(name.Substring(0, extensionIndex) + extension)
                : Parse(Path + extension);
        }

        [Pure]
        public int CompareTo(FilePath other)
        {
            return string.Compare(Path, other.Path, StringComparison.Ordinal);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (obj is FilePath path)
                return Equals(path);
            return false;
        }

        [Pure]
        public bool Equals(FilePath other)
        {
            return other.Path.Equals(Path);
        }

        [Pure]
        public string[] GetDirectorySegments()
        {
            var path = this;
            if (IsFile)
                path = path.ParentPath;
            var segments = new LinkedList<string>();
            while (!path.IsRoot)
            {
                segments.AddFirst(path.EntityName);
                path = path.ParentPath;
            }
            return segments.ToArray();
        }

        [Pure]
        public string GetExtension()
        {
            if (!IsFile)
                throw new ArgumentException("The specified FileSystemPath is not a file.");
            var name = EntityName;
            var extensionIndex = name.LastIndexOf('.');
            return extensionIndex < 0
                ? ""
                : name.Substring(extensionIndex);
        }

        [Pure]
        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        [Pure]
        public bool IsChildOf(FilePath path)
        {
            return path.IsParentOf(this);
        }

        [Pure]
        public bool IsParentOf(FilePath path)
        {
            return IsDirectory && Path.Length != path.Path.Length && path.Path.StartsWith(Path);
        }

        [Pure]
        public FilePath RemoveChild(FilePath child)
        {
            if (!Path.EndsWith(child.Path))
                throw new ArgumentException("The specified path is not a child of this path.");
            return new FilePath(Path.Substring(0, Path.Length - child.Path.Length + 1));
        }

        [Pure]
        public FilePath RemoveParent(FilePath parent)
        {
            if (!parent.IsDirectory)
                throw new ArgumentException("The specified path can not be the parent of this path: it is not a directory.");
            if (!Path.StartsWith(parent.Path))
                throw new ArgumentException("The specified path is not a parent of this path.");
            return new FilePath(Path.Remove(0, parent.Path.Length - 1));
        }

        [Pure]
        public override string ToString()
        {
            return Path;
        }
    }
}
