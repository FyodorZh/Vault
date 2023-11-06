using System.IO;

namespace Vault.Core
{
    public interface IContent
    {
        void WriteTo(TextWriter dst);
    }

    public class EmptyContent : IContent
    {
        public void WriteTo(TextWriter dst)
        {
        }
    }

    public class ValueContent<T> : IContent
    {
        public T Value { get; set; }

        public ValueContent(T value)
        {
            Value = value;
        }
        
        public void WriteTo(TextWriter dst)
        {
            dst.Write(Value.ToString());
        }
    }
}