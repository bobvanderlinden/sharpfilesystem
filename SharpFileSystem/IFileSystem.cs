using System.IO;
using System.Collections.Generic;
using System;

namespace SharpFileSystem
{
    public interface IFileSystem : IDisposable
    {
        void CreateDirectory(FilePath path);

        Stream CreateFile(FilePath path);

        void CreateTextFile(FilePath path, string contents);

        void Delete(FilePath path);

        bool Exists(FilePath path);

        FilePath GetCurrentDirectory();

        ICollection<FilePath> GetEntities(FilePath path);

        Stream OpenFile(FilePath path, FileAccess access);

        string ReadAllText(FilePath path);
    }
}
