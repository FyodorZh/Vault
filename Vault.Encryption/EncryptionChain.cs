using System;
using System.Collections.Generic;

namespace Vault.Encryption
{
    public interface IEncryptionChain
    {
        bool IsValid { get; }
        void Destroy();
        IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> data);
        IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> data);
    }

    public class VoidEncryptionChain : IEncryptionChain
    {
        public static readonly VoidEncryptionChain Instance = new VoidEncryptionChain();
        
        public bool IsValid => true;

        public void Destroy()
        {
        }

        public IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> data)
        {
            return data;
        }

        public IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> data)
        {
            return data;
        }
    }
    
    public class EncryptionChain : IEncryptionChain
    {
        private readonly IEncryptionChain? _base;
        private IEncryptionSource? _encryption;

        private bool SelfIsValid => _encryption is { NeedCredentials: false };

        public bool IsValid => SelfIsValid && (_base?.IsValid ?? true);

        public EncryptionChain(IEncryptionChain? @base, IEncryptionSource encryption)
        {
            _base = @base;
            _encryption = encryption;
        }

        public void Destroy()
        {
            _encryption = null;
        }

        public IReadOnlyList<byte> Encrypt(IReadOnlyList<byte> data)
        {
            if (!SelfIsValid)
            {
                throw new Exception();
            }

            data = _encryption!.Encrypt(data);
            return _base?.Encrypt(data) ?? data;
        }

        public IReadOnlyList<byte> Decrypt(IReadOnlyList<byte> data)
        {
            if (!SelfIsValid)
            {
                throw new Exception();
            }

            data = _base?.Decrypt(data) ?? data;
            return _encryption!.Decrypt(data);
        }
    }
}