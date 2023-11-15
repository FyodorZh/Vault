namespace Vault.Repository
{
    internal abstract class LockableAspect : ILockableAspect
    {
        private bool _unlocked;

        public bool IsLocked => !_unlocked;

        public virtual LockUnlockResult Unlock()
        {
            var res = _unlocked ? LockUnlockResult.NothingToDo : LockUnlockResult.Success;
            _unlocked = true;
            return res;
        }

        public virtual LockUnlockResult Lock()
        {
            var res = _unlocked ? LockUnlockResult.Success : LockUnlockResult.NothingToDo;
            _unlocked = false;
            return res;
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

        public sealed override LockUnlockResult Unlock()
        {
            if (_state != null)
            {
                return LockUnlockResult.NothingToDo;
            }
            
            _state = UnlockState();
            if (_state == null)
            {
                return LockUnlockResult.Fail;
            }
            
            base.Unlock();
            return LockUnlockResult.Success;
        }

        public sealed override LockUnlockResult Lock()
        {
            if (_state == null)
            {
                return LockUnlockResult.NothingToDo;
            }
            
            _state = null;
            LockState();
            base.Lock();
            return LockUnlockResult.Success;
        }
    }
}