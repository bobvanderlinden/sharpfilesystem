using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using SharpFileSystem.IO;
using SharpFileSystem.FileSystems;
using Xunit;

namespace SharpFileSystem.Tests.FileSystems
{
    public class NetZipArchiveFileSystemTest : IDisposable
    {
        private Stream zipStream;
        private NetZipArchiveFileSystem fileSystem;
        private NetZipArchiveFileSystem fileSystemReadOnly;
        private string fileContentString = "this is a file";

        //setup
        public  NetZipArchiveFileSystemTest()
        {
            zipStream = new FileStream("filesystem.zip", FileMode.Open);
            fileSystem = NetZipArchiveFileSystem.Open(zipStream);
            fileSystemReadOnly = NetZipArchiveFileSystem.OpenReadOnly(zipStream);
        }

        //teardown
        public void Dispose()
        {
            fileSystem.Dispose();
            zipStream.Dispose();
        }

        private readonly FileSystemPath directoryPath = FileSystemPath.Parse("/directory/");
        private readonly FileSystemPath textfileAPath = FileSystemPath.Parse("/textfileA.txt");
        private readonly FileSystemPath textfileBPath = FileSystemPath.Parse("/textfileB.txt");
        private readonly FileSystemPath fileInDirectoryPath = FileSystemPath.Parse("/directory/fileInDirectory.txt");
        private readonly FileSystemPath scratchDirectoryPath = FileSystemPath.Parse("/scratchdirectory/");

        [Fact]
        public void GetEntitiesOfRootTest()
        {
            Assert.Equal(new[]
            {
                directoryPath,
                scratchDirectoryPath,
                textfileAPath,
                textfileBPath
            }, fileSystem.GetEntities(FileSystemPath.Root).ToArray());
        }

        [Fact]
        public void GetEntitiesOfDirectoryTest()
        {
            Assert.Equal(new[]
            {
                fileInDirectoryPath
            }, fileSystem.GetEntities(directoryPath).ToArray());
        }

        [Fact]
        public void ExistsTest()
        {
            Assert.True(fileSystem.Exists(FileSystemPath.Root));
            Assert.True(fileSystem.Exists(textfileAPath));
            Assert.True(fileSystem.Exists(directoryPath));
            Assert.True(fileSystem.Exists(fileInDirectoryPath));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/nonExistingFile")));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/nonExistingDirectory/")));
            Assert.False(fileSystem.Exists(FileSystemPath.Parse("/directory/nonExistingFileInDirectory")));
        }

        [Fact]
        public void CanReadFile()
        {
            var file = fileSystem.OpenFile(textfileAPath, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.True(string.Equals(text, fileContentString));
        }

        [Fact]
        public void CanWriteFile()
        {
            var file = fileSystem.OpenFile(textfileBPath, FileAccess.ReadWrite);
            var textBytes = Encoding.ASCII.GetBytes(fileContentString + " and a new string");
            file.Write(textBytes);
            file.Close();


            file = fileSystem.OpenFile(textfileBPath, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.True(string.Equals(text, fileContentString + " and a new string"));
        }

        [Fact]
        public void CanAddFile()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/recentlyadded.txt");
            var file = fileSystem.CreateFile(fsp);
            var textBytes = Encoding.ASCII.GetBytes("recently added");
            file.Write(textBytes);
            file.Close();

            Assert.True(fileSystem.Exists(fsp));

            file = fileSystem.OpenFile(fsp, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.True(string.Equals(text, "recently added"));
        }

        [Fact]
        public void CanAddDirectory()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/dir/");
            fileSystem.CreateDirectory(fsp);

            Assert.True(fileSystem.Exists(fsp));
        }

        [Fact]
        public void ReadOnlyGetEntitiesOfRootTest()
        {
            Assert.Equal(new[]
            {
                directoryPath,
                scratchDirectoryPath,
                textfileAPath,
                textfileBPath
            }, fileSystemReadOnly.GetEntities(FileSystemPath.Root).ToArray());
        }

        [Fact]
        public void ReadOnlyGetEntitiesOfDirectoryTest()
        {
            Assert.Equal(new[]
            {
                fileInDirectoryPath
            }, fileSystemReadOnly.GetEntities(directoryPath).ToArray());
        }

        [Fact]
        public void ReadOnlyExistsTest()
        {
            Assert.True(fileSystemReadOnly.Exists(FileSystemPath.Root));
            Assert.True(fileSystemReadOnly.Exists(textfileAPath));
            Assert.True(fileSystemReadOnly.Exists(directoryPath));
            Assert.True(fileSystemReadOnly.Exists(fileInDirectoryPath));
            Assert.False(fileSystemReadOnly.Exists(FileSystemPath.Parse("/nonExistingFile")));
            Assert.False(fileSystemReadOnly.Exists(FileSystemPath.Parse("/nonExistingDirectory/")));
            Assert.False(fileSystemReadOnly.Exists(FileSystemPath.Parse("/directory/nonExistingFileInDirectory")));
        }

        [Fact]
        public void ReadOnlyCanReadFile()
        {
            var file = fileSystemReadOnly.OpenFile(textfileAPath, FileAccess.Read);
            var text = file.ReadAllText();
            Assert.True(string.Equals(text, fileContentString));
        }

        [Fact]
        public void ReadOnlyCanNotWriteFile()
        {
            Assert.Throws<InvalidOperationException>(() => fileSystemReadOnly.OpenFile(textfileBPath, FileAccess.ReadWrite));
        }

        [Fact]
        public void ReadOnlyCanNotAddFile()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/recentlyadded.txt");
            Assert.Throws<InvalidOperationException>(() => fileSystemReadOnly.CreateFile(fsp));
        }

        [Fact]
        public void ReadOnlyCanNotAddDirectory()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/dir/");
            Assert.Throws<InvalidOperationException>(() => fileSystemReadOnly.CreateDirectory(fsp));
        }
    }
}
