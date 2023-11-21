using System.IO;
using OrderedSerializer;

namespace Vault.Scripting
{
    public interface IResult : IDataStruct
    {
        void WriteTo(IOutputTextStream dst);
    }
    
    public abstract class Result : IResult, IVersionedDataStruct
    {
        public abstract void WriteTo(IOutputTextStream dst);
        
        public abstract void Serialize(IOrderedSerializer serializer);

        public byte Version => 0;
    }
}