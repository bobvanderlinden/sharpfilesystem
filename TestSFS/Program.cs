using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using Xunit;

namespace TestSFS
{
    class Program
    {


        static void Main(string[] args)
        {
            embeddedFS();
            CreateMemoryFile();
            ReadZipFS();
            WriteZipFS();
            // Read7ZipFS();

        }

        static void CreateMemoryFile()
        {
            FileSystemPath MemRootFilePath = FileSystemPath.Root.AppendFile("x");
            var MemFileSystem = new MemoryFileSystem();
            // File shouldn’t exist prior to creation.
            Assert.False(MemFileSystem.Exists(MemRootFilePath));

            var content = new byte[] { 0xde, 0xad, 0xbe, 0xef, };
            using (var xStream = MemFileSystem.CreateFile(MemRootFilePath))
            {
                // File now should exist.
                Assert.True(MemFileSystem.Exists(MemRootFilePath));

                xStream.Write(content, 0, content.Length);
            }

            // File should still exist and have content.
            Assert.True(MemFileSystem.Exists(MemRootFilePath));
            using (var xStream = MemFileSystem.OpenFile(MemRootFilePath, FileAccess.Read))
            {
                var readContent = new byte[2 * content.Length];
                Assert.Equal(content.Length, xStream.Read(readContent, 0, readContent.Length));
                Assert.Equal(
                    content,
                    // trim to the length that was read.
                    readContent.Take(content.Length).ToArray());

                // Trying to read beyond end of file should return 0.
                Assert.Equal(0, xStream.Read(readContent, 0, readContent.Length));
            }
        }


        static void embeddedFS()
        {
            string content = "test embedded resource";
            string deepContent = "deep file";
            var filePath = FileSystemPath.Root.AppendFile("test.txt");
            var resDir = FileSystemPath.Root.AppendDirectory("resDir");
            var deepFilePath = resDir.AppendFile("deepFile.txt");
            EmbeddedResourceFileSystem eRscFS = new EmbeddedResourceFileSystem(Assembly.GetAssembly(typeof(Program)));
            Assert.True(eRscFS.Exists(filePath));
            using (var stream = eRscFS.OpenFile(filePath,FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    string value = reader.ReadToEnd();
                    Assert.Equal(content,value);
                }
            }

            Assert.True(eRscFS.Exists(deepFilePath));
            using (var stream = eRscFS.OpenFile(deepFilePath,FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    string value = reader.ReadToEnd();
                    Assert.Equal(deepContent,value);
                }
            }

            var entities = eRscFS.GetEntities(FileSystemPath.Root);
            var recentities = eRscFS.GetEntitiesRecursive(FileSystemPath.Root);
            ;
        }

        static void WriteZipFS()
        {
            var dirPath = FileSystemPath.Root.AppendDirectory("dir");
            var filePath = dirPath.AppendFile("file.txt");

            string zipFileName = @".\newtest.zip";
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

        static void ReadZipFS()
                {
                    FileSystemPath MemRootFilePath = FileSystemPath.Root.AppendFile("x");
                    var zipFileSystem = SharpZipLibFileSystem.Open(System.IO.File.Open(@".\test.zip",FileMode.Open));
                    // File shouldn’t exist prior to creation.
                    Assert.False(zipFileSystem.Exists(MemRootFilePath));

                    // File should still exist and have content.
                    var file = FileSystemPath.Parse("/file");
                    Assert.True(zipFileSystem.Exists(file));
                    using (var xStream = zipFileSystem.OpenFile(file,FileAccess.Read))
                    {
                        var readContent = new byte[128];
                        int read =xStream.Read(readContent, 0, readContent.Length);

                        Assert.Equal(4,read);
                        string value = Encoding.ASCII.GetString(readContent,0,read);
                        Assert.Equal("test",value);
                        // Trying to read beyond end of file should return 0.
                        Assert.Equal(0, xStream.Read(readContent, 0, readContent.Length));
                    }
                }


    }
}
