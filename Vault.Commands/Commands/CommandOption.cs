using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Commands
{
    [Guid("09E21DCD-51BD-4DEB-98A7-28FF6236A617")]
    public struct CommandOption : IVersionedDataStruct
    {
        private string? _name;
        private string? _parameter;

        public string Name => _name ?? "";
        public string? Parameter => _parameter;

        public CommandOption(string name, string? parameter = null)
        {
            _name = name;
            _parameter = parameter;
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _name);
            serializer.Add(ref _parameter);
        }

        public byte Version => 0;
    }
}