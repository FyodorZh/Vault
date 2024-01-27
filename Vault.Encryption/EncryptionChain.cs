using System;
using System.Collections.Generic;

namespace Vault.Encryption
{
    public interface IEncryptionChain
    {
        bool IsValid { get; }
        void Destroy();
        IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> data);
        IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> data);
    }

    public class VoidEncryptionChain : IEncryptionChain
    {
        public static readonly VoidEncryptionChain Instance = new VoidEncryptionChain();
        
        public bool IsValid => true;

        public void Destroy()
        {
        }

        public IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> data)
        {
            return data;
        }

        public IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> data)
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

        public IReadOnlyList<byte>? Encrypt(IReadOnlyList<byte> data)
        {
            if (!SelfIsValid)
            {
                throw new Exception();
            }

            var encryptedData = _encryption!.Encrypt(data);
            if (encryptedData == null)
            {
                return null;
            }
            return _base?.Encrypt(encryptedData) ?? encryptedData;
        }

        public IReadOnlyList<byte>? Decrypt(IReadOnlyList<byte> data)
        {
            if (!SelfIsValid)
            {
                throw new Exception();
            }

            IReadOnlyList<byte>? planeData;
            if (_base != null)
            {
                planeData = _base.Decrypt(data);
                if (planeData == null)
                {
                    return null;
                }
            }
            else
            {
                planeData = data;
            }
            
            planeData = _encryption!.Decrypt(planeData);
            return planeData;
        }
    }
}