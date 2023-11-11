using System.IO;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Encryption;

namespace Vault.Content
{
    [Guid("BB70D6DB-AC6A-43BB-910E-F259642732FB")]
    public class DirectoryContent : IContent
    {
        private EncryptionSource? _names;
        private EncryptionSource? _content;

        public DirectoryContent()
        {
        }
        
        public DirectoryContent(EncryptionSource? names, EncryptionSource? content)
        {
            _names = names;
            _content = content;
        }

        public EncryptionSource? GetForNames() => _names ?? _content;

        public EncryptionSource? GetForContent() => _content;

        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.AddClass(ref _names);
            serializer.AddClass(ref _content);
        }

        public byte Version => 0;
        
        public void WriteTo(TextWriter dst)
        {
            if (_names == null && _content == null)
            {
                dst.WriteLine("Encryption: NONE");
                return;
            }
            
            dst.WriteLine("Encryption:");
                
            if (_names != null)
            {
                dst.WriteLine("- Names: ");
                WriteEncryptionDesc(dst, _names.GetDescription());
            }

            if (_content != null)
            {
                dst.WriteLine("- Content: ");
                WriteEncryptionDesc(dst, _content.GetDescription());
            }
        }
        
        private static void WriteEncryptionDesc(TextWriter dst, EncryptionDesc encryptionDesc)
        {
            dst.Write($"Method='{encryptionDesc.MethodName}' ");
            if (encryptionDesc.RequireCredentials)
            {
                dst.Write($"Credentials={(encryptionDesc.HasCredentials ? "PRESENT" : "ABSENT")}");
            }
            dst.WriteLine();
        }
    }
}