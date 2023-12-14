using Vault.Repository;
using Vault.Storage;

namespace Vault.Commands
{
    public interface IProcessorContext
    {
        IRepository Repository { get; }
        IStorage Storage { get; }
        
        ICredentialsProvider CredentialsProvider { get; }
        void SetCredentialsInAdvance(CredentialsType type, string credentials);
        
        IDirectoryNode Current { get; set; }
    }
}