using System;
using SharpFileSystem.FileSystems;
using System.IO;
using System.Linq;
using System.Text;
using SharpFileSystem.IO;
using SharpFileSystem.SharpZipLib;
using Xunit;


namespace SharpFileSystem.Tests.FileSystems
{
    public class ZipFileSystemTests
    {
        [Fact]
        void WriteZipFSTest()
        {
            var dirPath = FileSystemPath.Root.AppendDirectory("dir");
            var filePath = dirPath.AppendFile("file.txt");

            string zipFileName = @"./newtest.zip";
            if (System.IO.File.Exists(zipFileName))
            {
                System.IO.File.Delete(zipFileName);
            }

            using (var zipFileSystem =
                SharpZipLibFileSystem.Open(System.IO.File.Open(zipFileName, FileMode.OpenOrCreate)))
            {
                using (var transaction = zipFileSystem.OpenWriteTransaction())
                {
                    zipFileSystem.CreateDirectory(dirPath);
                    using (var stream = zipFileSystem.CreateFile(filePath))
                    {
                        using (StreamWriter sw = new StreamWriter(stream))
                        {
                            sw.Write("hello zip");
                        }
                    }
                }

                ;
                // TODO : check if written sucessfully

                Assert.True(zipFileSystem.Exists(filePath));
                using (var xStream = zipFileSystem.OpenFile(filePath, FileAccess.Read))
                {
                    var readContent = new byte[128];
                    int read = xStream.Read(readContent, 0, readContent.Length);

                    Assert.Equal(9, read);
                    string value = Encoding.ASCII.GetString(readContent, 0, read);
                    Assert.Equal("hello zip", value);
                    // Trying to read beyond end of file should return 0.
                    Assert.Equal(0, xStream.Read(readContent, 0, readContent.Length));
                }
            }
        }

        [Fact]
        void ReadZipFS()
        {
            FileSystemPath MemRootFilePath = FileSystemPath.Root.AppendFile("x");
            var zipFileSystem = SharpZipLibFileSystem.Open(System.IO.File.Open(@"./test.zip", FileMode.Open));
            // File shouldn’t exist prior to creation.
            Assert.False(zipFileSystem.Exists(MemRootFilePath));

            // File should still exist and have content.
            var file = FileSystemPath.Parse("/file");
            Assert.True(zipFileSystem.Exists(file));
            using (var xStream = zipFileSystem.OpenFile(file, FileAccess.Read))
            {
                var readContent = new byte[128];
                int read = xStream.Read(readContent, 0, readContent.Length);

                Assert.Equal(4, read);
                string value = Encoding.ASCII.GetString(readContent, 0, read);
                Assert.Equal("test", value);
                // Trying to read beyond end of file should return 0.
                Assert.Equal(0, xStream.Read(readContent, 0, readContent.Length));
            }
        }
    }
}
