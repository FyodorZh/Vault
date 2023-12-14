using System;
using System.Collections.Generic;
using System.IO;
using Vault.Encryption;
using Vault.Repository;
using Vault.Repository.V1;
using Vault.Storage;

namespace Vault.Commands
{
    public class CommandsProcessor : IProcessorContext, ICredentialsProvider
    {
        private readonly Queue<(CredentialsType, string)> _credentialsQueue = new Queue<(CredentialsType, string)>();

        private readonly Func<string?> _credentialsProvider;
        private readonly TextWriter _humanOutputStream;

        public IStorage Storage { get; }
        
        public IRepository Repository { get; }

        public ICredentialsProvider CredentialsProvider => this;

        public void SetCredentialsInAdvance(CredentialsType type, string credentials)
        {
            _credentialsQueue.Enqueue((type, credentials));
        }

        public IDirectoryNode Current { get; set; }

        public CommandsProcessor(IStorage storage, Func<string?> credentialsProvider, TextWriter humanOutputStream)
        {
            Storage = storage;
            Repository = new RepositoryV1(storage, this);
            _credentialsProvider = credentialsProvider;
            _humanOutputStream = humanOutputStream;
            Current = Repository.GetRoot();
        }

        string? ICredentialsProvider.GetCredentials(IDirectoryNode dir, CredentialsType credentialsType, EncryptionDesc encryptionDesc)
        {
            if (_credentialsQueue.Count > 0)
            {
                var qPair = _credentialsQueue.Dequeue();
                if ((qPair.Item1 & credentialsType) == credentialsType)
                {
                    return qPair.Item2;
                }
                _humanOutputStream.Write("Error: wrong queued credentials type");
                _credentialsQueue.Clear();
            }
            
            _humanOutputStream.Write("Enter credentials for " + credentialsType + ": ");
            return _credentialsProvider();
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