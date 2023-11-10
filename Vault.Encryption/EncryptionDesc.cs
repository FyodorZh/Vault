namespace Vault.Encryption
{
    public record EncryptionDesc
    {
        public string MethodName;
        public bool RequireCredentials;
        public bool HasCredentials;
        
        public EncryptionDesc(string methodName, bool requireCredentials, bool hasCredentials)
        {
            MethodName = methodName;
            RequireCredentials = requireCredentials;
            HasCredentials = hasCredentials;
        }
    }
}