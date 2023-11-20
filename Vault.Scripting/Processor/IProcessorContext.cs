using Vault.Repository;
using Vault.Storage;

namespace Vault.Scripting
{
    public interface IProcessorContext
    {
        IRepository Repository { get; }
        IStorage Storage { get; }
        ICredentialsProvider CredentialsProvider { get; }
        
        IDirectoryNode Current { get; set; }
    }
}