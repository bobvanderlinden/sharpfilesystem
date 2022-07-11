using System;
using System.Collections.Generic;
using System.IO;
using SharpFileSystem.FileSystems;
using Xunit;

namespace SharpFileSystem.Tests.FileSystems
{
    public class FSMounterTests
    {
        [Fact]
        public void TestMount()
        {
            var memFs = new MemoryFileSystem();
            var embedFS = new EmbeddedResourceFileSystem(typeof(MergeFSTests).Assembly);

            var mounter = new FileSystemMounter(new KeyValuePair<FileSystemPath, IFileSystem>("/memory/",memFs), new KeyValuePair<FileSystemPath, IFileSystem>("/embed/",embedFS));

            Assert.True(mounter.Exists("/embed/resDir/deepFile.txt"));
            Assert.Throws<NotSupportedException>(() => mounter.OpenFile("/embed/toto.txt", FileAccess.Write));

            using (var stream = mounter.CreateFile("/memory/test.txt"))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("hello");
                }
            }

            Assert.True(mounter.Exists("/memory/test.txt"));

            var embedEntities = mounter.GetEntities("/embed/");
            Assert.Equal(2,embedEntities.Count);
            var memoryEntities = mounter.GetEntities("/memory/");
            Assert.Single(memoryEntities);
        }
    }
}
