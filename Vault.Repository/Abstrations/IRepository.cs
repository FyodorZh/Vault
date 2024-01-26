using System.Threading.Tasks;

namespace Vault.Repository
{
    public interface IRepository
    {
        Task<IDirectoryNode> GetRoot();
    }
}