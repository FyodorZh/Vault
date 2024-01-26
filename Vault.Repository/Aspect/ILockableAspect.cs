using System.Threading.Tasks;

namespace Vault.Repository
{
    public enum LockUnlockResult : byte
    {
        Unknown,
        NothingToDo,
        Success,
        Fail
    }
    
    public interface ILockableAspect
    {
        bool IsLocked { get; }
        LockUnlockResult Unlock();
        Task<LockUnlockResult> Lock();
    }
    
    public interface ILockableAspect<out T> : ILockableAspect
    {
        T? Value { get; }
    }
}