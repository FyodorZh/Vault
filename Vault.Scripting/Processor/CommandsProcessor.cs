using System;
using System.IO;
using Vault.Encryption;
using Vault.Repository;
using Vault.Repository.V1;
using Vault.Storage;

namespace Vault.Scripting
{
    public class CommandsProcessor : IProcessorContext, ICredentialsProvider
    {
        public IStorage Storage { get; }
        
        public IRepository Repository { get; }

        public ICredentialsProvider CredentialsProvider => this;

        public IDirectoryNode Current { get; set; }

        public TextWriter HumanOutput { get; private set; }

        public CommandsProcessor(IStorage storage)
        {
            Storage = storage;
            Repository = new RepositoryV1(storage, this);
            Current = Repository.GetRoot();
            HumanOutput = Console.Out;
        }

        string? ICredentialsProvider.GetCredentials(IDirectoryNode dir, EncryptionDesc encryptionDesc, string text)
        {
            Console.Write("Enter credentials for " + text + ": ");
            return Console.ReadLine();
        }

        public Result Process(ICommand cmd)
        {
            try
            {
                return cmd.Process(this);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex);
            }
        }
    }
}