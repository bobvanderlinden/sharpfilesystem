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
    public class SharpZipLibFileSystemWithPasswordTest
    {
        private Stream zipStream;
        private SharpZipLibFileSystem fileSystem;
        private SharpZipLibFileSystem badFileSystem;
        private readonly string zipPassword = "unittestpassword";
        private readonly string fileContent = "this is a file";
        [OneTimeSetUp]
        public void Initialize()
        {
            CreateInitialZipStream();
            fileSystem = SharpZipLibFileSystem.Open(zipStream,zipPassword);
            badFileSystem = SharpZipLibFileSystem.Open(zipStream, "wrongpassword");
        }

        private void CreateInitialZipStream()
        {
            var memoryStream = new MemoryStream();
            zipStream = memoryStream;
            var zipOutput = new ZipOutputStream(zipStream);
            zipOutput.Password = zipPassword;

            var fileContentString = fileContent;
            var fileContentBytes = Encoding.ASCII.GetBytes(fileContentString);
            zipOutput.PutNextEntry(new ZipEntry("textfileA.txt")
            {
                Size = fileContentBytes.Length
            });
            zipOutput.Write(fileContentBytes);
            zipOutput.PutNextEntry(new ZipEntry("directory/fileInDirectory.txt"));
            zipOutput.Finish();

            memoryStream.Position = 0;

        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            fileSystem.Dispose();
            badFileSystem.Dispose();
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
        public void CanOpenFileWithCorrectPassword()
        {
            var fs = fileSystem.OpenFile(textfileAPath, FileAccess.Read);
            Assert.AreEqual(fs.ReadAllText(), fileContent);
        }

        [Test]
        public void OpenFileThrowsZipExceptiontWithIncorrectPassword()
        {
            Assert.Throws<ZipException>( () =>
            {
                badFileSystem.OpenFile(textfileAPath, FileAccess.Read);
            });
        }
    }
}
