using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using SharpFileSystem.IO;
using SharpFileSystem.SharpZipLib;

namespace SharpFileSystem.Tests.SharpZipLib
{
    [TestFixture]
    public class SharpZipLibFileSystemTest
    {
        private Stream zipStream;
        private SharpZipLibFileSystem fileSystem;
        private MemoryStream memoryStream;
        [OneTimeSetUp]
        public void Initialize()
        {
            memoryStream = new MemoryStream();
            zipStream = memoryStream;
            var zipOutput = new ZipOutputStream(zipStream);
            zipOutput.UseZip64 = UseZip64.Off;

            var fileContentString = "this is a file";
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContentString);
            zipOutput.PutNextEntry(new ZipEntry("textfileA.txt")
            {
                Size = fileContentBytes.Length
            });
            zipOutput.Write(fileContentBytes);
            zipOutput.PutNextEntry(new ZipEntry("directory/fileInDirectory.txt"));
            zipOutput.Finish();

            memoryStream.Position = 0;
            fileSystem = SharpZipLibFileSystem.Open(zipStream);
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

        [Test]
        public void GetEntitiesOfRootTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                textfileAPath,
                directoryPath
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
        public void CanCreateFileTest()
        {
            var text = "recent text";
            var textBytes = Encoding.ASCII.GetBytes(text);
            var fsp = FileSystemPath.Parse("/mostrecentfile.txt");
            var fs = fileSystem.CreateFile(fsp, textBytes);
            fs.Close();
            Assert.IsTrue(fileSystem.Exists(fsp));

            fs = fileSystem.OpenFile(fsp, FileAccess.Read);
            var fsText = fs.ReadAllText();

            Assert.IsTrue(fsText.Equals(text));

            fileSystem.Delete(fsp);
        }

        [Test]
        public void EmbeddedZipTest()
        {
            var lms = new MemoryStream();
            var lzs = lms;
            var zipOutput = new ZipOutputStream(lzs);
            zipOutput.UseZip64 = UseZip64.Off;

            var fileContentString = "this is a file";
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContentString);
            zipOutput.PutNextEntry(new ZipEntry("textfileA.txt")
            {
                Size = fileContentBytes.Length
            });
            zipOutput.Write(fileContentBytes);
            zipOutput.PutNextEntry(new ZipEntry("directory/fileInDirectory.txt"));
            zipOutput.Finish();

            lms.Position = 0;

            var zipBytes = lzs.ReadAllBytes();
            var fsp = FileSystemPath.Parse("/mostrecentfile.zip");
            var fs = fileSystem.CreateFile(fsp, zipBytes);
            fs.Close();
            Assert.IsTrue(fileSystem.Exists(fsp));

            var zipFile = fileSystem.OpenFile(fsp, FileAccess.Read);

            var zipFileSystem = SharpZipLibFileSystem.Open(zipFile);

            Assert.IsNotNull(zipFileSystem);
            //cleanup so we don't affect other unit tests
            fileSystem.Delete(fsp);
        }
    }
}
