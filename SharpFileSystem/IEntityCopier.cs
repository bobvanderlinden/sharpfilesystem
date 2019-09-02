namespace SharpFileSystem
{
    public interface IEntityCopier
    {
        void Copy(IFileSystem source, FilePath sourcePath, IFileSystem destination, FilePath destinationPath);
    }
}
