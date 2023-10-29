namespace Vault.Repository.InMemory
{
    public class Node : INode
    {
        public Guid Id { get; }
        public string Name { get; }

        public IDirectoryNode? Parent { get; }

        public Node(DirectoryNode? parent, string name)
        {
            Id = Guid.NewGuid();
            Parent = parent;
            Name = name;
        }

        public void Dispose()
        {
        }
    }
}