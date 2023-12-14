using Vault.Encryption;

namespace Vault.Repository
{
    public interface ICredentialsProvider
    {
        string? GetCredentials(IDirectoryNode dir, CredentialsType credentialsType, EncryptionDesc encryptionDesc);
    }
}