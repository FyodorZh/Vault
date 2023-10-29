namespace Vault.Repository.InFileSystem
{
    public class Node : INode
    {
        public Guid Id { get; }
        public string Name { get; }

        public IDirectoryNode? Parent { get; }

        public Node(DirectoryNode? parent, string name)
        {
            if (parent != null)
            {
                throw new NotImplementedException();
                //parent.AddChild(this);
            }
            Parent = parent;
            Name = name;
        }

        public void Dispose()
        {
        }
    }
}