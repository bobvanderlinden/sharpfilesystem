using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpFileSystem.SharpZipLib;

namespace SharpFileSystem.Tests.SharpZipLib
{
    [TestClass]
    public class SharpZipLibFileSystemTest
    {
        private FileStream fileStream;
        private SharpZipLibFileSystem fileSystem;

        [TestInitialize]
        public void Initialize()
        {
            fileStream = System.IO.File.OpenRead("SharpZipLib/Content/test.zip");
            fileSystem = SharpZipLibFileSystem.Open(fileStream);
        }

        [TestCleanup]
        public void Cleanup()
        {
            fileSystem.Dispose();
            fileStream.Dispose();
        }

        private readonly FileSystemPath directoryPath = FileSystemPath.Parse("/directory/");
        private readonly FileSystemPath textfileAPath = FileSystemPath.Parse("/textfileA.txt");
        private readonly FileSystemPath fileInDirectoryPath = FileSystemPath.Parse("/directory/fileInDirectory.txt");

        [TestMethod]
        public void GetEntitiesOfRootTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                textfileAPath,
                directoryPath
            }, fileSystem.GetEntities(FileSystemPath.Root).ToArray());
        }

        [TestMethod]
        public void GetEntitiesOfDirectoryTest()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                fileInDirectoryPath
            }, fileSystem.GetEntities(directoryPath).ToArray());
        }

        [TestMethod]
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
    }
}
