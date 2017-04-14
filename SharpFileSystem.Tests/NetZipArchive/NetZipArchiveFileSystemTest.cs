using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using SharpFileSystem.IO;
using SharpFileSystem.SharpZipArchive;

namespace SharpFileSystem.Tests.NetZipArchive
{
    [TestFixture]
    public class NetZipArchiveFileSystemTest
    {
        private Stream zipStream;
        private NetZipArchiveFileSystem fileSystem;
        private string fileContentString = "this is a file";
        [OneTimeSetUp]
        public void Initialize()
        {
            var memoryStream = new MemoryStream();
            zipStream = memoryStream;
            var zipOutput = new ZipOutputStream(zipStream);

            
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContentString);
            zipOutput.PutNextEntry(new ZipEntry("textfileA.txt")
            {
                Size = fileContentBytes.Length
            });
            zipOutput.Write(fileContentBytes);
            zipOutput.PutNextEntry(new ZipEntry("directory/fileInDirectory.txt"));
            zipOutput.PutNextEntry(new ZipEntry("scratchdirectory/scratch"));
            zipOutput.Finish();

            memoryStream.Position = 0;
            fileSystem = NetZipArchiveFileSystem.Open(zipStream);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            fileSystem.Dispose();
            zipStream.Dispose();
        }

        private readonly FileSystemPath directoryPath = FileSystemPath.Parse("/directory/");
        private readonly FileSystemPath textfileAPath = FileSystemPath.Parse("/textfileA.txt");
        private readonly FileSystemPath fileInDirectoryPath = FileSystemPath.Parse("/directory/fileInDirectory.txt");
        private readonly FileSystemPath scratchDirectoryPath = FileSystemPath.Parse("/scratchdirectory/");

        [Test]
        public void GetEntitiesOfRootTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                textfileAPath,
                directoryPath,
                scratchDirectoryPath
            }, fileSystem.GetEntities(FileSystemPath.Root).ToArray());
        }

        [Test]
        public void GetEntitiesOfDirectoryTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                fileInDirectoryPath
            }, fileSystem.GetEntities(directoryPath).ToArray());
        }

        [Test]
        public void ExistsTest()
        {
            Assert.IsTrue(fileSystem.Exists(FileSystemPath.Root));
            Assert.IsTrue(fileSystem.Exists(textfileAPath));
            Assert.IsTrue(fileSystem.Exists(directoryPath));
            Assert.IsTrue(fileSystem.Exists(fileInDirectoryPath));
            Assert.IsFalse(fileSystem.Exists(FileSystemPath.Parse("/nonExistingFile")));
            Assert.IsFalse(fileSystem.Exists(FileSystemPath.Parse("/nonExistingDirectory/")));
            Assert.IsFalse(fileSystem.Exists(FileSystemPath.Parse("/directory/nonExistingFileInDirectory")));
        }

        [Test]
        public void CanReadFile()
        {
            var file = fileSystem.OpenFile(textfileAPath, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.IsTrue(string.Equals(text, fileContentString));
        }

        [Test]
        public void CanWriteFile()
        {
            var file = fileSystem.OpenFile(textfileAPath, FileAccess.ReadWrite);
            var textBytes = Encoding.ASCII.GetBytes(fileContentString + " and a new string");
            file.Write(textBytes);
            file.Close();


            file = fileSystem.OpenFile(textfileAPath, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.IsTrue(string.Equals(text, fileContentString + " and a new string"));
        }

        [Test]
        public void CanAddFile()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/recentlyadded.txt");
            var file = fileSystem.CreateFile(fsp);
            var textBytes = Encoding.ASCII.GetBytes("recently added");
            file.Write(textBytes);
            file.Close();

            Assert.IsTrue(fileSystem.Exists(fsp));

            file = fileSystem.OpenFile(fsp, FileAccess.ReadWrite);
            var text = file.ReadAllText();
            Assert.IsTrue(string.Equals(text, "recently added"));
        }

        [Test]
        public void CanAddDirectory()
        {
            var fsp = FileSystemPath.Parse("/scratchdirectory/newdir/");
            fileSystem.CreateDirectory(fsp);

            Assert.IsTrue(fileSystem.Exists(fsp));
        }
    }
}
