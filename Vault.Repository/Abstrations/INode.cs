using System;
using System.Collections.Generic;
using Vault.Encryption;

namespace Vault.Repository
{
    [Flags]
    public enum LockState
    {
        Open = 0,
        Name = 1,
        Content = 2,
        Closed = Name | Content, 
        All = Closed
    }

    public interface INode
    {
        bool IsValid { get; }
        Guid Id { get; }
        
        LockState State { get; }
        void Unlock(LockState stateChange);
        void Lock(LockState stateChange);
        
        string? Name { get; }
        
        IDirectoryNode? Parent { get; }
    }
}