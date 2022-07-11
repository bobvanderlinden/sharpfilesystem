using System.IO;
using SharpFileSystem.FileSystems;
using Xunit;

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
            Assert.Equal(3,entities.Count);


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
            Assert.Equal("hello embed !",content);
        }
    }
}
