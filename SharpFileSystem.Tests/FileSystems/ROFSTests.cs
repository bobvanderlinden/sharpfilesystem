using System;
using System.IO;
using SharpFileSystem.FileSystems;
using Xunit;

namespace SharpFileSystem.Tests.FileSystems
{
    public class ROFSTests
    {
        [Fact]
        public void TestROFS()
        {
            var memFs = new MemoryFileSystem();
            memFs.CreateDirectory("/mem/");
            using (var stream = memFs.CreateFile("/mem/test.txt"))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("hello");
                }
            }
            var rofs = new ReadOnlyFileSystem(memFs);

            Assert.Throws<InvalidOperationException>(() => rofs.CreateDirectory("/memory/"));
            Assert.Throws<InvalidOperationException>(() => rofs.CreateFile("/memory/test.txt"));
            var content = rofs.ReadAllText("/mem/test.txt");
            Assert.Equal("hello",content);

        }
    }
}
