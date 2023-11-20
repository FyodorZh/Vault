using OrderedSerializer;

namespace Vault.Scripting
{
    public abstract class Result : IVersionedDataStruct
    {
        public abstract void Serialize(IOrderedSerializer serializer);

        public byte Version => 0;
    }
}