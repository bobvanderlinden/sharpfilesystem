using System.IO;
using System.Reflection;
using SharpFileSystem.Resources;
using Xunit;


namespace SharpFileSystem.Tests.FileSystems
{
    public class EmbeddedRessourceFileSystemTests
    {
        [Fact]
        void eResFSTest()
        {
            string content = "test embedded resource";
            string deepContent = "deep file";
            var filePath = FileSystemPath.Root.AppendFile("test.txt");
            var resDir = FileSystemPath.Root.AppendDirectory("resDir");
            var deepFilePath = resDir.AppendFile("deepFile.txt");
            EmbeddedResourceFileSystem eRscFS = new EmbeddedResourceFileSystem(Assembly.GetAssembly(typeof(EmbeddedRessourceFileSystemTests)));
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
            Assert.Equal(2,entities.Count);
            var recentities = eRscFS.GetEntitiesRecursive(FileSystemPath.Root);
            ;
        }
    }
}
