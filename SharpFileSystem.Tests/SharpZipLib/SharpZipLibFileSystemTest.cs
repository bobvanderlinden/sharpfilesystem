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
        private readonly FilePath directoryPath = FilePath.Parse("/directory/");
        private readonly FilePath fileInDirectoryPath = FilePath.Parse("/directory/fileInDirectory.txt");
        private readonly FilePath textfileAPath = FilePath.Parse("/textfileA.txt");
        private SharpZipLibFileSystem fileSystem;
        private Stream zipStream;
        [OneTimeTearDown]
        public void Cleanup()
        {
            fileSystem.Dispose();
            zipStream.Dispose();
        }

        [Test]
        public void ExistsTest()
        {
            Assert.IsTrue(fileSystem.Exists(FilePath.Root));
            Assert.IsTrue(fileSystem.Exists(textfileAPath));
            Assert.IsTrue(fileSystem.Exists(directoryPath));
            Assert.IsTrue(fileSystem.Exists(fileInDirectoryPath));
            Assert.IsFalse(fileSystem.Exists(FilePath.Parse("/nonExistingFile")));
            Assert.IsFalse(fileSystem.Exists(FilePath.Parse("/nonExistingDirectory/")));
            Assert.IsFalse(fileSystem.Exists(FilePath.Parse("/directory/nonExistingFileInDirectory")));
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
        public void GetEntitiesOfRootTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                textfileAPath,
                directoryPath
            }, fileSystem.GetEntities(FilePath.Root).ToArray());
        }

        [OneTimeSetUp]
        public void Initialize()
        {
            var memoryStream = new MemoryStream();
            zipStream = memoryStream;
            var zipOutput = new ZipOutputStream(zipStream);

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
    }
}
