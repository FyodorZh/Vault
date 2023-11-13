using Vault.Content;

namespace Vault.Repository.V1
{
    internal abstract class ContentState<TContent> : LockedState<IContent, TContent>
        where TContent : class, IContent
    {
        private readonly Node _owner;

        protected abstract bool UnlockContent(TContent content);
            
        protected ContentState(Node node) 
            : base(true)
        {
            _owner = node;
        }

        protected sealed override TContent? UnlockState()
        {
            IContent? c = _owner.Data.EncryptedContent.Deserialize(_owner.Parent?.Encryption.ContentEncryptionChain);
            if (c == null)
            {
                return null;
            }
            TContent content = (TContent)c;
            if (UnlockContent(content))
            {
                return content;
            }

            return null;
        }
    }
}