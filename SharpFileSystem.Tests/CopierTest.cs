using System.IO;
using SharpFileSystem.FileSystems;
using Xunit;

namespace SharpFileSystem.Tests
{
    public class CopierTest
    {
        [Fact]
        public void TestCopy()
        {
            var memFs1 = new MemoryFileSystem();
            var memFs2 = new MemoryFileSystem();

            memFs1.CreateDirectory("/fs1/");
            memFs1.CreateDirectory("/fs1/memory/");
            using (var stream = memFs1.CreateFile("/fs1/memory/test.txt"))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write("hello");
                }
            }

            memFs2.CreateDirectory("/fs2/");
            memFs2.CreateDirectory("/fs2/memory/");
            var copier = new StandardEntityCopier();
            copier.Copy(memFs1, "/fs1/memory/test.txt", memFs2,"/fs2/memory/text.txt");
            Assert.True(memFs2.Exists("/fs2/memory/text.txt"));
            var content = memFs2.ReadAllText("/fs2/memory/text.txt");
            Assert.Equal("hello",content);


        }
    }
}
