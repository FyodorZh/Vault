using System.Text;

namespace Vault.Encryption
{
    public class StringBox : EncryptedBox<string>
    {
        public StringBox(string encryptedData) 
            : base(encryptedData)
        {
        }

        protected override string Deserialize(string data)
        {
            return data;
        }

        protected override string Serialize(string data)
        {
            return data;
        }
    }
}