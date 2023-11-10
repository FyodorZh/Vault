using System;
using OrderedSerializer;

namespace Vault.Content
{
    public struct NodeId : IEquatable<NodeId>, IVersionedDataStruct
    {
        private bool _isValid;
        private Guid _guid;

        public bool IsValid => _isValid;

        public static readonly NodeId Invalid = new NodeId();

        public NodeId(Guid id)
        {
            _isValid = true;
            _guid = id;
        }

        public static NodeId NewId()
        {
            return new NodeId(Guid.NewGuid());
        }

        public override string ToString()
        {
            return _isValid ? _guid.ToString() : "invalid";
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            if (serializer.IsWriter)
            {
                serializer.Add(ref _isValid);
                if (_isValid)
                {
                    var guid = _guid.ToString();
                    serializer.Add(ref guid);
                }
            }
            else
            {
                _isValid = false;
                
                bool isValid = false;
                serializer.Add(ref isValid);
                if (isValid)
                {
                    string? guid = null;
                    serializer.Add(ref guid);
                    _guid = Guid.Parse(guid!);
                    _isValid = true;
                }
                else
                {
                    _guid = new Guid();
                }
            }
        }

        public byte Version => 0;

        public bool Equals(NodeId other)
        {
            if (_isValid != other._isValid)
            {
                return false;
            }

            if (_isValid)
            {
                return _guid.Equals(other._guid);
            }

            return true;
        }

        public override bool Equals(object? obj)
        {
            return obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _isValid ? _guid.GetHashCode() : 0;
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