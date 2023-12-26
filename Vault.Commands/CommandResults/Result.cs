using System.IO;
using Archivarius;

namespace Vault.Commands
{
    public interface IResult : IDataStruct
    {
        void WriteTo(IOutputTextStream dst);
    }
    
    public abstract class Result : IResult, IVersionedDataStruct
    {
        public abstract void WriteTo(IOutputTextStream dst);
        
        public abstract void Serialize(ISerializer serializer);

        public byte Version => 0;
    }
}