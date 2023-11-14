namespace Vault.Repository
{
    public interface ILockableAspect
    {
        bool IsLocked { get; }
        bool Unlock();
        void Lock();
    }
    
    public interface ILockableAspect<out T> : ILockableAspect
    {
        T? Value { get; }
    }
}