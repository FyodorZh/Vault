using System.IO;
using OrderedSerializer;

namespace Vault.Content
{
    public interface IContent : IVersionedDataStruct
    {
        void WriteTo(TextWriter dst);
    }

    public interface IContent<T> : IContent
    {
        T Content { get; }
    }
    
    public abstract class Content : IContent
    {
        public abstract void WriteTo(TextWriter dst);
        
        public abstract void Serialize(IOrderedSerializer serializer);

        public virtual byte Version => 0;
    }
}