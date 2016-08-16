using System;
using SharpFileSystem.FileSystems;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpFileSystem.IO;

namespace SharpFileSystem.Tests.FileSystems
{
    [TestFixture]
    public class PhysicalFileSystemTest
    {
        string Root { get; set; }
        PhysicalFileSystem FileSystem { get; set; }
        string AbsoluteFileName { get; set; }

        string FileName { get; }
        FileSystemPath FileNamePath { get; }

        public PhysicalFileSystemTest()
        {
            FileName = "x";
            FileNamePath = FileSystemPath.Root.AppendFile(FileName);
        }

        [SetUp]
        public void Initialize()
        {
            Root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(Root);
            AbsoluteFileName = Path.Combine(Root, FileName);
            FileSystem = new PhysicalFileSystem(Root);
        }

        [TearDown]
        public void Cleanup()
        {
            using (FileSystem) { }
            System.IO.Directory.Delete(Root, true);
        }

        [Test]
        public void CreateFile()
        {
            Assert.IsFalse(System.IO.File.Exists(AbsoluteFileName));
            Assert.IsFalse(FileSystem.Exists(FileNamePath));

            var content = Encoding.UTF8.GetBytes("asdf");
            using (var stream = FileSystem.CreateFile(FileNamePath))
            {
                // File should exist at this point.
                Assert.IsTrue(FileSystem.Exists(FileNamePath));
                // File should also exist irl at this point.
                Assert.IsTrue(System.IO.File.Exists(AbsoluteFileName));

                stream.Write(content, 0, content.Length);
            }

            // File should contain content.
            CollectionAssert.AreEqual(content, System.IO.File.ReadAllBytes(AbsoluteFileName));

            using (var stream = FileSystem.OpenFile(FileNamePath, FileAccess.Read))
            {
                // Verify that EOF type stuff works.
                var readContent = new byte[2 * content.Length];
                Assert.AreEqual(content.Length, stream.Read(readContent, 0, readContent.Length));
                CollectionAssert.AreEqual(
                    content,
                    // trim to actual length.
                    readContent.Take(content.Length).ToArray());

                // Trying to read beyond end of file should just return 0.
                Assert.AreEqual(0, stream.Read(readContent, 0, readContent.Length));
            }
        }

        [Test]
        public void CreateFile_Exists()
        {
            Assert.IsFalse(System.IO.File.Exists(AbsoluteFileName));
            Assert.IsFalse(FileSystem.Exists(FileNamePath));

            using (var stream = FileSystem.CreateFile(FileNamePath))
            {
                var content1 = Encoding.UTF8.GetBytes("asdf");
                stream.Write(content1, 0, content1.Length);
            }

            // creating an existing file should truncate like open(O_CREAT).
            var content2 = Encoding.UTF8.GetBytes("b");
            using (var stream = FileSystem.CreateFile(FileNamePath))
            {
                stream.Write(content2, 0, content2.Length);
            }
            CollectionAssert.AreEqual(content2, System.IO.File.ReadAllBytes(AbsoluteFileName));
            using (var stream = FileSystem.OpenFile(FileNamePath, FileAccess.Read))
            {
                CollectionAssert.AreEqual(content2, stream.ReadAllBytes());
            }
        }

        [Test]
        public void CreateFile_Empty()
        {
            using (var stream = FileSystem.CreateFile(FileNamePath))
            {
            }

            CollectionAssert.AreEqual(
                new byte[] { },
                System.IO.File.ReadAllBytes(AbsoluteFileName));
            using (var stream = FileSystem.OpenFile(FileNamePath, FileAccess.Read))
            {
                CollectionAssert.AreEqual(
                    new byte[] { },
                    stream.ReadAllBytes());
            }
        }
    }
}
