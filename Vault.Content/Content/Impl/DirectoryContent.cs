using System;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Encryption;

namespace Vault.Content
{
    public interface IDirectoryContent : IContent
    {
        IEncryptionSource GetForNames();
        IEncryptionSource GetForContent();
    }
    
    [Guid("BB70D6DB-AC6A-43BB-910E-F259642732FB")]
    public class DirectoryContent : IDirectoryContent
    {
        private EncryptionSource? _namesAndContent;
        private EncryptionSource? _names;
        private EncryptionSource? _content;

        private DirectoryContent()
        {
        }

        public DirectoryContent(EncryptionSource namesAndContent)
        {
            _namesAndContent = namesAndContent;
        }
        
        public DirectoryContent(EncryptionSource names, EncryptionSource content)
        {
            if (names == _content)
            {
                throw new InvalidOperationException();
            }
            _names = names;
            _content = content;
        }

        public IEncryptionSource GetForNames() => _namesAndContent ?? _names ?? throw new InvalidOperationException();

        public IEncryptionSource GetForContent() => _namesAndContent ?? _content ?? throw new InvalidOperationException();

        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.AddClass(ref _namesAndContent);
            serializer.AddClass(ref _names);
            serializer.AddClass(ref _content);
        }

        public byte Version => 0;
    }
}