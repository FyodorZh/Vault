using System.Runtime.InteropServices;
using OrderedSerializer;

namespace Vault.Encryption
{
    [Guid("2B5E26BE-249C-4274-B612-070C4AFC9A0B")]
    public struct EncryptionDesc : IVersionedDataStruct
    {
        private string? _methodName;
        private bool _requireCredentials;
        private bool _hasCredentials;
        
        public string MethodName => _methodName ?? "";
        public bool RequireCredentials => _requireCredentials;
        public bool HasCredentials => _hasCredentials;

        public EncryptionDesc(string methodName, bool requireCredentials, bool hasCredentials)
        {
            _methodName = methodName;
            _requireCredentials = requireCredentials;
            _hasCredentials = hasCredentials;
        }

        public override string ToString()
        {
            return $"Method={_methodName ?? "???"} RequireCredentials={_requireCredentials} HasCredentials={_hasCredentials}";
        }

        public void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _methodName);
            serializer.Add(ref _requireCredentials);
            serializer.Add(ref _hasCredentials);
        }

        public byte Version => 0;
    }
}