namespace Vault.Encryption
{
    public interface IDecryptor
    {
        string Decrypt(string encryptedData);
    }
}