using System.Runtime.InteropServices;
using Vault.Encryption;

namespace Vault.Commands
{
    [Guid("C8C3113A-F10D-4A2F-B3C3-EBD23B97E1D2")]
    public class EncryptCommand : Command2
    {
        public override string Name => "encrypt";

        private EncryptCommand()
        {
        }
        
        public EncryptCommand(string? nameEncryptionType, string? contentEncryptionType)
            : base(new CommandOption("name", nameEncryptionType), 
                new CommandOption("content", contentEncryptionType))
        {}
        
        private static bool EncryptionFactory(string name, out EncryptionSource? encryptionSource)
        {
            switch (name)
            {
                case "null":
                case "nul":
                case "none":
                    encryptionSource = null;
                    return true;
                case "plain":
                    encryptionSource = new PlaneDataEncryptionSource();
                    return true;
                case "xor":
                    encryptionSource = new XorEncryptionSource();
                    return true;
                default:
                    encryptionSource = null;
                    return false;
            }
        }

        public override Result Process(IProcessorContext context)
        {
            if (!EncryptionFactory(Options[0].Parameter!, out var nameEncryption) ||
                !EncryptionFactory(Options[1].Parameter!, out var contentEncryption))
            {
                return Fail();
            }

            if (nameEncryption is { NeedCredentials: true })
            {
                string? credential = context.CredentialsProvider.GetCredentials(context.Current, nameEncryption.GetDescription(), "name");
                if (credential == null)
                {
                    return Fail();
                }
                nameEncryption.AddCredentials(credential);
            }
            if (contentEncryption is { NeedCredentials: true })
            {
                string? credential = context.CredentialsProvider.GetCredentials(context.Current, contentEncryption.GetDescription(), "content");
                if (credential == null)
                {
                    return Fail();
                }
                contentEncryption.AddCredentials(credential);
            }

            context.Current.SetEncryption(nameEncryption, contentEncryption);

            return Ok;
        }
    }
}