using System.IO;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Scripting
{
    [Guid("B64C8B3A-B5A7-4ED3-9D1D-4D402F3AC872")]
    public class OkResult : Result
    {
        public OkResult()
        {
        }

        public override void WriteTo(IOutputTextStream dst)
        {
            // DO NOTHING
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
        }
    }
}