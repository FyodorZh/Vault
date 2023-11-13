namespace Vault.Repository.V1
{
    internal class NameState : LockedState<string, string>
    {
        private readonly Node _owner;
        
        internal NameState(Node owner)
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