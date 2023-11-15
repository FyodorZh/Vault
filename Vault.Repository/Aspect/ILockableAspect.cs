namespace Vault.Repository
{
    public enum LockUnlockResult
    {
        NothingToDo,
        Success,
        Fail
    }
    
    public interface ILockableAspect
    {
        bool IsLocked { get; }
        LockUnlockResult Unlock();
        LockUnlockResult Lock();
    }
    
    public interface ILockableAspect<out T> : ILockableAspect
    {
        T? Value { get; }
    }
}