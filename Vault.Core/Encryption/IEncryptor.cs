namespace Vault.Encryption
{
    public interface IEncryptor
    {
        string Encrypt(string plainData);
    }
}