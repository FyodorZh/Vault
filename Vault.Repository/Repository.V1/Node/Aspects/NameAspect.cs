namespace Vault.Repository.V1
{
    internal class NameAspect : LockableAspect<string, string>
    {
        private readonly Node _owner;
        
        internal NameAspect(Node owner)
            : base(true)
        {
            _owner = owner;
        }
        
        protected override string? UnlockState()
        {
            return _owner.Data.EncryptedName.Deserialize(_owner.Parent?.Encryption.ChildrenNameEncryptionChain)?.Content;
        }
    }
}