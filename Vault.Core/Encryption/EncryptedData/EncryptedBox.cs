namespace Vault.Encryption
{
    public abstract class EncryptedBox<T>
    {
        private string _encryptedData;
        private T? _plainData;
        
        public bool IsDecrypted { get; private set; }

        protected abstract T Deserialize(string data);
        protected abstract string Serialize(T data);

        public EncryptedBox(string encryptedData)
        {
            _encryptedData = encryptedData;
            IsDecrypted = false;
        }

        public void Decrypt(IDecryptor decryptor)
        {
            var decrypted = decryptor.Decrypt(_encryptedData);
            _plainData = Deserialize(decrypted);
            IsDecrypted = true;
        }

        public void Set(T newData, IEncryptor encryptor)
        {
            var serialized = Serialize(newData);
            _encryptedData = encryptor.Encrypt(serialized);
            _plainData = newData;
            IsDecrypted = true;
        }

        public void Wipe()
        {
            IsDecrypted = false;
            _plainData = default;
        }
    }
}