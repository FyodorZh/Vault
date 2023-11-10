using System;
using Vault.Content;

namespace Vault.Repository
{
    [Flags]
    public enum LockState
    {
        Open = 0,
        SelfName = 1,               // имя ноды
        ChildrenName = 2,           // имена чилдов ноды (если папка)
        Content = 4,                // для файлов контент, для папки контент всех чилдов
        Closed = SelfName | ChildrenName | Content, 
        All = Closed
    }

    public interface INode
    {
        bool IsValid { get; }
        NodeId Id { get; }
        
        LockState State { get; }
        void Unlock(LockState stateChange);
        void Lock(LockState stateChange);
        
        string? Name { get; }
        IDirectoryNode? Parent { get; }
    }
}