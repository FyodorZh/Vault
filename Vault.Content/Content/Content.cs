using System.IO;
using OrderedSerializer;

namespace Vault.Content
{
    public abstract class Content : IContent
    {
        public abstract void Serialize(IOrderedSerializer serializer);

        public virtual byte Version => 0;
    }
}