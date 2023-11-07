using System;
using System.Collections.Generic;
using Vault.Encryption;

namespace Vault.Repository
{
    public interface INode
    {
        bool IsValid { get; }
        Guid Id { get; }
        
        string? Name { get; }
        bool DecryptName(IEnumerable<Decryptor>? decryptorsChain = null);
        
        IDirectoryNode? Parent { get; }
    }
}