using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SharpFileSystem.FileSystems
{

    public static class AssemblyExtensions
    {
        public static string GetShortName(this Assembly assembly)
        {
            return assembly.FullName.Split(new[] {','}).First();
        }
    }
    public class EmbeddedResourceFileSystem : AbstractFileSystem
    {
        public Assembly Assembly { get; private set; }


        public override bool IsReadOnly => true;

        private string AssemblyName => Assembly.GetShortName();
        public EmbeddedResourceFileSystem(Assembly assembly)
        {
            Assembly = assembly;
        }

        public override ICollection<FileSystemPath> GetEntities(FileSystemPath path)
        {
            if (!path.IsRoot)
                throw new DirectoryNotFoundException();
            var entities = new List<FileSystemPath>();
            var resources = Assembly.GetManifestResourceNames();

            foreach (var resource in resources)
            {
                var res = resource.Replace(AssemblyName + ".", "");
                var elements = res.Split(new[] { '.' });
                var resourcePath = elements.Take(elements.Length - 2);
                var entityPath = "/"+string.Join("/", resourcePath) + elements[elements.Length - 2] + "." + elements[elements.Length - 1];
                entities.Add(entityPath);
            }

            return entities;
            //return Assembly.GetManifestResourceNames().Select(name => FileSystemPath.Root.AppendFile(name.Replace(AssemblyName+".",""))).ToArray();
        }

        public override bool Exists(FileSystemPath path)
        {
            return path.IsRoot || !path.IsDirectory && Assembly.GetManifestResourceNames().Contains($"{AssemblyName}.{path.Path.Substring(1).Replace("/",".")}");
        }

        public override Stream OpenFile(FileSystemPath path, FileAccess access)
        {
            if (access == FileAccess.Write)
                throw new NotSupportedException();
            // if (path.IsDirectory || path.ParentPath != FileSystemPath.Root)
            //     throw new FileNotFoundException();
            return Assembly.GetManifestResourceStream($"{AssemblyName}.{path.Path.Substring(1).Replace("/",".")}");
        }

        public override Stream CreateFile(FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public override void CreateDirectory(FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public override void Delete(FileSystemPath path)
        {
            throw new NotSupportedException();
        }



        public override void Dispose()
        {
        }
    }
}
