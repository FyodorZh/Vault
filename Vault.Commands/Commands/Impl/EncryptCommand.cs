using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Archivarius;
using Vault.Encryption;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("C8C3113A-F10D-4A2F-B3C3-EBD23B97E1D2")]
    public class EncryptCommand : Command
    {
        public override string Name => "encrypt";

        private string? _nameAndContentEncryptionType;
        private string? _nameEncryptionType;
        private string? _contentEncryptionType;

        private EncryptCommand()
        {
        }

        public EncryptCommand(string nameAndContentEncryptionType)
        {
            _nameAndContentEncryptionType = nameAndContentEncryptionType;
        }

        public EncryptCommand(string nameEncryptionType, string contentEncryptionType)
        {
            _nameEncryptionType = nameEncryptionType;
            _contentEncryptionType = contentEncryptionType;
        }
        
        private static bool EncryptionFactory(string name, out EncryptionSource? encryptionSource)
        {
            switch (name)
            {
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

        public override async Task<Result> Process(IProcessorContext context)
        {
            EncryptionSource? nameAndContentEncryption = null;
            EncryptionSource? nameEncryption = null;
            EncryptionSource? contentEncryption = null;

            if (_nameAndContentEncryptionType != null)
                EncryptionFactory(_nameAndContentEncryptionType, out nameAndContentEncryption);
            if (_nameEncryptionType != null)
                EncryptionFactory(_nameEncryptionType, out nameEncryption);
            if (_contentEncryptionType != null)
                EncryptionFactory(_contentEncryptionType, out contentEncryption);
            
            if (nameAndContentEncryption == null && 
                (nameEncryption == null || contentEncryption == null))
            {
                return await Fail();
            }
            
            if (nameAndContentEncryption is { NeedCredentials: true })
            {
                string? credential = context.CredentialsProvider.GetCredentials(context.Current, CredentialsType.NamesAndContent, nameAndContentEncryption.GetDescription());
                if (credential == null)
                {
                    return await Fail();
                }
                nameAndContentEncryption.AddCredentials(credential);
            }
            if (nameEncryption is { NeedCredentials: true })
            {
                string? credential = context.CredentialsProvider.GetCredentials(context.Current, CredentialsType.Names, nameEncryption.GetDescription());
                if (credential == null)
                {
                    return await Fail();
                }
                nameEncryption.AddCredentials(credential);
            }
            if (contentEncryption is { NeedCredentials: true })
            {
                string? credential = context.CredentialsProvider.GetCredentials(context.Current, CredentialsType.Content, contentEncryption.GetDescription());
                if (credential == null)
                {
                    return await Fail();
                }
                contentEncryption.AddCredentials(credential);
            }

            bool isOk;
            if (nameAndContentEncryption != null)
            {
                isOk = await context.Current.SetEncryption(nameAndContentEncryption, nameAndContentEncryption);
            }
            else
            {
                isOk = await context.Current.SetEncryption(nameEncryption!, contentEncryption!);
            }

            return await (isOk ? Ok : Fail());
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _nameAndContentEncryptionType);
            serializer.Add(ref _nameEncryptionType);
            serializer.Add(ref _contentEncryptionType);
        }
    }
}