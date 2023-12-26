using System;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.FileSystem
{
    public interface IEntityName
    {
        bool IsRoot { get; }
        IEntityName? Parent { get; }
        string Name { get; }
        string FullName { get; }
    }
    
    [Guid("DB8D4134-3A0E-4B15-9673-B99FD5E8E4E1")]
    public sealed class EntityName : IEquatable<EntityName>, IVersionedDataStruct
    {
        public static readonly EntityName Root = new EntityName();
        
        private EntityName? _parent;
        private string _name;
        
        private int _depth;
        private string? _fullName;
        
        public bool IsRoot => _parent == null;

        public string Name => _name;

        public int Depth => _depth;

        public string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    if (_parent != null)
                    {
                        _fullName = _parent.FullName + "/" + _name;
                    }
                    else
                    {
                        _fullName = _name;
                    } 
                }

                return _fullName;
            }
        }

        public EntityName()
        {
            _parent = null;
            _name = "";
            _depth = 0;
        }

        public EntityName(EntityName parent, string name)
        {
            _parent = parent;
            _name = name;
            _depth = parent.Depth + 1;
        }

        public bool IsSubEntity(EntityName subEntityName, bool directChild = true)
        {
            if (directChild && Depth + 1 != subEntityName.Depth ||
                !directChild && Depth >= subEntityName.Depth)
            {
                return false;
            }
            
            string thisName = FullName;
            string otherName = subEntityName.FullName;

            if (otherName.StartsWith(thisName) &&
                otherName[thisName.Length] == '/')
            {
                return true;
            }

            return false;
        }

        public bool Equals(EntityName? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return _name == other._name && _parent == other._parent;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is EntityName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_parent?.GetHashCode() ?? 0) ^ _name.GetHashCode();
        }

        public static bool operator ==(EntityName? left, EntityName? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityName? left, EntityName? right)
        {
            return !Equals(left, right);
        }

        public void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _name, () => throw new Exception());
            serializer.AddClass(ref _parent);
            if (!serializer.IsWriter)
            {
                _depth = _parent != null ? (_parent.Depth + 1) : 0;
                _fullName = null;
            }
        }

        public byte Version => 0;
    }
}