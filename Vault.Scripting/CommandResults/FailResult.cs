using System.IO;
using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Scripting
{
    [Guid("32313464-8606-4ADA-BE93-B92D9E32C349")]
    public class FailResult : Result
    {
        private string? _message;

        public string Message => _message ?? "";
        
        public FailResult()
        {}

        public FailResult(string? message)
        {
            _message = message;
        }

        public override void WriteTo(IOutputTextStream dst)
        {
            dst.Write("Error: ");
            dst.WriteLine(_message ?? "???");
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _message);
        }
    }
}