using System;
using System.Collections.Generic;
using Vault.Content;
using Vault.Encryption;

namespace Vault.Repository
{
    [Flags]
    public enum LockState
    {
        Open = 0,
        SelfName = 1,               // имя ноды
        Content = 4,                // для файлов контент, для папки контент всех чилдов
        Closed = SelfName | Content, 
        All = Closed
    }

    public interface INode
    {
        bool IsValid { get; }
        NodeId Id { get; }
        
        LockState State { get; }

        void LockAll();
        
        bool UnlockName();
        void LockName();
        
        bool UnlockContent();
        void LockContent();

        string? Name { get; }
        IContent? Content { get; }
        
        IDirectoryNode? Parent { get; }
    }
}