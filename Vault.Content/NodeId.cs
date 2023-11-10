using System;
using OrderedSerializer;

namespace Vault.Content
{
    public struct NodeId : IEquatable<NodeId>, IVersionedDataStruct
    {
        private Guid _guid;

        public NodeId(Guid id)
        {
            _guid = id;
        }

        public static NodeId NewId()
        {
            return new NodeId(Guid.NewGuid());
        }

        public override string ToString()
        {
            return _guid.ToString();
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            if (serializer.IsWriter)
            {
                var guid = _guid.ToString();
                serializer.Add(ref guid);
            }
            else
            {
                string? guid = null;
                serializer.Add(ref guid);
                _guid = Guid.Parse(guid!);
            }
        }

        public byte Version => 0;

        public bool Equals(NodeId other)
        {
            return _guid.Equals(other._guid);
        }

        public override bool Equals(object? obj)
        {
            return obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
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