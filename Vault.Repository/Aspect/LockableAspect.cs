using System;

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

        private bool _gateLock;

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
            try
            {                
                if (_state != null)
                {
                    return LockUnlockResult.NothingToDo;
                }
                
                if (_gateLock)
                {
                    throw new Exception();
                }
                _gateLock = true;

                _state = UnlockState();
                if (_state == null)
                {
                    return LockUnlockResult.Fail;
                }

                base.Unlock();
                return LockUnlockResult.Success;
            }
            finally
            {
                _gateLock = false;
            }
        }

        public sealed override LockUnlockResult Lock()
        {
            try
            {
                if (_state == null)
                {
                    return LockUnlockResult.NothingToDo;
                }
                _state = null;
                base.Lock();
                
                if (_gateLock)
                {
                    throw new Exception();
                }
                _gateLock = true;
                

                LockState();
                
                return LockUnlockResult.Success;
            }
            finally
            {
                _gateLock = false;
            }
        }
    }
}