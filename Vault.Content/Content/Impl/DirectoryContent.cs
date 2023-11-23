using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Encryption;

namespace Vault.Content
{
    public interface IDirectoryContent : IContent
    {
        IEncryptionSource? GetForNames();
        IEncryptionSource? GetForContent();
    }
    
    [Guid("BB70D6DB-AC6A-43BB-910E-F259642732FB")]
    public class DirectoryContent : IDirectoryContent
    {
        private EncryptionSource? _names;
        private EncryptionSource? _content;

        private DirectoryContent()
        {
        }
        
        public DirectoryContent(EncryptionSource? names = null, EncryptionSource? content = null)
        {
            _names = names;
            _content = content;
        }

        public IEncryptionSource? GetForNames() => _names ?? _content;

        public IEncryptionSource? GetForContent() => _content;

        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.AddClass(ref _names);
            serializer.AddClass(ref _content);
        }

        public byte Version => 0;
    }
}