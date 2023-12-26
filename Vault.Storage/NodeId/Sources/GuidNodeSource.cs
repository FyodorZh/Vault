using System;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Storage
{
    [Guid("DF69FF26-4F64-4B23-8BE8-1B94CFC1359D")]
    public class GuidNodeIdSource : INodeIdSource
    {
        public void Serialize(ISerializer serializer)
        {
            // DO NOTHING
        }

        public byte Version => 0;
        
        public NodeId GenNew()
        {
            return new NodeId(Guid.NewGuid().ToString());
        }
    }
}