using System.Collections.Generic;

namespace Vault.FileSystem
{
    public abstract class Validity
    {
        public delegate void InvalidateEvent(bool wasInvalidBeforehand);
        
        public abstract bool IsValid { get; }
        
        public static implicit operator bool(Validity validity)
        {
            return validity.IsValid;
        }

        public abstract void CallIfInvalidated(InvalidateEvent evt);
    }
    
    public class ValidityImpl : Validity
    {
        private bool _isValid = true;
        private List<InvalidateEvent>? _subscribers;

        public override bool IsValid => _isValid;

        public override void CallIfInvalidated(InvalidateEvent evt)
        {
            if (!_isValid)
            {
                evt(true);
            }

            _subscribers ??= new List<InvalidateEvent>();
            _subscribers.Add(evt);
        }

        public bool Invalidate()
        {
            if (_isValid)
            {
                _isValid = false;
                if (_subscribers != null)
                {
                    foreach (var evt in _subscribers)
                    {
                        evt.Invoke(false);
                    }

                    _subscribers = null;
                }
                return true;
            }

            return false;
        }
    }
}