using System;
using System.IO;
using Vault.Encryption;
using Vault.Repository;
using Vault.Repository.V1;
using Vault.Storage;

namespace Vault.Scripting
{
    public class RepositoryProcessor : IProcessorContext, ICredentialsProvider
    {
        public IStorage Storage { get; }
        
        public IRepository Repository { get; }

        public ICredentialsProvider CredentialsProvider => this;

        public IDirectoryNode Current { get; set; }

        public TextWriter HumanOutput { get; private set; }

        public RepositoryProcessor(IStorage storage)
        {
            Storage = storage;
            Repository = new RepositoryV1(storage, this);
            Current = Repository.GetRoot();
            HumanOutput = Console.Out;
            Prompt();
        }

        string? ICredentialsProvider.GetCredentials(IDirectoryNode dir, EncryptionDesc encryptionDesc, string text)
        {
            Console.Write("Enter credentials for " + text + ": ");
            return Console.ReadLine();
        }

        public Result Process(Command cmd)
        {
            try
            {
                return cmd.Process(this);
            }
            catch (Exception ex)
            {
                return new ExceptionResult(ex);
            }
            finally
            {
                Prompt();    
            }
        }
        
        public void Prompt()
        {
            string prompt = Current.Name;
            INode? c = Current.Parent;
            while (c != null)
            {
                prompt = c.Name + "/" + prompt;
                c = c.Parent;
            }

            HumanOutput.Write(prompt + "> ");
        }
    }
}