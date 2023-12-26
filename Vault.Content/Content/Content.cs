using System.IO;
using Archivarius;

namespace Vault.Content
{
    public abstract class Content : IContent
    {
        public abstract void Serialize(ISerializer serializer);

        public virtual byte Version => 0;
    }
}