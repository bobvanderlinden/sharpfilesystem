namespace SharpFileSystem
{
    public interface IEntityMover
    {
        void Move(IFileSystem source, FilePath sourcePath, IFileSystem destination, FilePath destinationPath);
    }
}
