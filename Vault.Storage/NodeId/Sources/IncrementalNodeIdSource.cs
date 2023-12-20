using System;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Storage
{
    [Guid("CB3F1BF6-B7C3-4EFB-848B-2B4CF006C69B")]
    public class IncrementalNodeIdSource : INodeIdSource
    {
        private long _nextId = 1;
        
        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _nextId);
        }

        public byte Version => 0;
        
        public NodeId GenNew()
        {
            return new NodeId("id_" + (_nextId++));
        }
    }
}