namespace Vault.Repository
{
    public interface INode : IDisposable
    {
        Guid Id { get; }
        //bool NameDecoded { get; }
        string Name { get; }
        IDirectoryNode? Parent { get; }
    }
}