using Vault.Encryption;

namespace Vault.Repository
{
    public interface ICredentialsProvider
    {
        string? GetCredentials(IDirectoryNode dir, EncryptionDesc encryptionDesc, string text);
    }
}