using System;

namespace Vault.Repository
{
    public interface ILockedState<out T>
    {
        bool Unlocked { get; }
        T? Value { get; }
        bool Unlock();
        void Lock();
    }
    
    internal abstract class LockedState<T, InternalT> : ILockedState<T>
        where T : class
        where InternalT : class, T
    {
        private readonly bool _autoUnlock;
        
        private InternalT? _state;

        public bool Unlocked => _state != null;

        public T? Value
        {
            get
            {
                if (_state == null && _autoUnlock)
                {
                    Unlock();
                }
                return _state;
            }
        }

        protected LockedState(bool autoUnlock)
        {
            _autoUnlock = autoUnlock;
        }

        protected abstract InternalT? UnlockState();

        protected virtual void LockState() { }

        public bool Unlock()
        {
            _state ??= UnlockState();
            return _state != null;
        }
        
        public void Lock()
        {
            _state = null;
            LockState();
        }
    }
}