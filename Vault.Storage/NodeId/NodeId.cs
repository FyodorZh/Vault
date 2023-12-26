using System;
using Archivarius;

namespace Vault.Storage
{
    public struct NodeId : IEquatable<NodeId>, IDataStruct
    {
        private string? _id;

        public bool IsValid => _id != null;

        public static readonly NodeId Invalid = new NodeId();

        public NodeId(string id)
        {
            _id = id;
        }

        public void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _id);
        }

        public override string ToString()
        {
            return _id ?? "";
        }

        public bool Equals(NodeId other)
        {
            return _id == other._id;
        }

        public override bool Equals(object? obj)
        {
            return obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_id != null ? _id.GetHashCode() : 0);
        }

        public static bool operator ==(NodeId left, NodeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NodeId left, NodeId right)
        {
            return !left.Equals(right);
        }
    }
}