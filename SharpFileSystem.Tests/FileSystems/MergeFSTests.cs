using System.Collections.Generic;
using System.IO;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SharpFileSystem.FileSystems;
using Xunit;
using Assert = Xunit.Assert;

namespace SharpFileSystem.Tests.FileSystems
{
    public class MergeFSTests
    {
        [Fact]
        public void TestMerge()
        {
            var memFs = new MemoryFileSystem();
            var embedFS = new EmbeddedResourceFileSystem(typeof(MergeFSTests).Assembly);
            var merge = new MergedFileSystem(memFs, embedFS);
            merge.CreateDirectory("/memory/");
            using (var stream = merge.CreateFile("/memory/test.txt"))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("hello");
                }
            }


            Assert.True(merge.Exists("/resDir/deepFile.txt"));
            Assert.True(merge.Exists("/memory/test.txt"));
            var content = merge.ReadAllText("/memory/test.txt");
            Assert.Equal("hello",content);
            content = merge.ReadAllText("/resDir/deepFile.txt");
            Assert.Equal("deep file", content);

            var entities = merge.GetEntities("/");
            Assert.Equal(5,entities.Count);


            merge.CreateDirectory("/resDir/");
            using (var stream = merge.CreateFile("/resDir/memoryFileThatMatchAnEmbeddedPath.txt"))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("hello embed !");
                }
            }
            Assert.True(merge.Exists("/resDir/memoryFileThatMatchAnEmbeddedPath.txt"));
            content = merge.ReadAllText("/resDir/memoryFileThatMatchAnEmbeddedPath.txt");
            Check.That(content).IsEqualTo("hello embed !");


            var files = merge.GetFiles("/resDir/");
            var expectedFiles = new List<FileSystemPath>()
                { "/resDir/deepFile.txt", "/resDir/memoryFileThatMatchAnEmbeddedPath.txt" };
            Check.That(files).IsEquivalentTo(expectedFiles);


            var directories = merge.GetDirectories("/");
            var expectedDirectories = new List<FileSystemPath>() { "/memory", "/resDir" };
            Check.That(directories).IsEquivalentTo(expectedDirectories);


            embedFS.ChRoot("/resDir");

            files = merge.GetFiles("/");
            expectedFiles = new List<FileSystemPath>() { "/deepFile.txt" };
            Check.That(files).IsEquivalentTo(expectedFiles);

            directories = merge.GetDirectories("/");
            expectedDirectories = new List<FileSystemPath>() { "/deep","/memory","/resDir" };
            Check.That(directories).IsEquivalentTo(expectedDirectories);
        }
    }
}
