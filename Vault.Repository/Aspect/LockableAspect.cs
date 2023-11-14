namespace Vault.Repository
{
    internal abstract class LockableAspect : ILockableAspect
    {
        private bool _unlocked;

        public bool IsLocked => !_unlocked;

        public virtual bool Unlock()
        {
            _unlocked = true;
            return true;
        }

        public virtual void Lock()
        {
            _unlocked = false;
        }
    }

    internal abstract class LockableAspect<T, InternalT> : LockableAspect, ILockableAspect<T>
        where T : class
        where InternalT : class, T
    {
        private readonly bool _autoUnlock;

        private InternalT? _state;

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

        protected LockableAspect(bool autoUnlock)
        {
            _autoUnlock = autoUnlock;
        }

        protected abstract InternalT? UnlockState();

        protected virtual void LockState()
        {
        }

        public sealed override bool Unlock()
        {
            _state ??= UnlockState();
            if (_state != null)
            {
                return base.Unlock();
            }

            base.Lock();
            return false;
        }

        public sealed override void Lock()
        {
            _state = null;
            LockState();
            base.Lock();
        }
    }
}