using System;
using System.Collections.Generic;

namespace Vault.Serialization
{
    public static class DefaultTypeSet
    {
        private static bool _used = false;
        
        private static Func<int, IReadOnlyList<Type>>? _defaultTypeSetProvider;
        private static int _defaultTypeSetVersion;

        public static void Setup(int defaultTypeSetVersion, Func<int, IReadOnlyList<Type>>? defaultTypeSetProvider)
        {
            if (_used)
            {
                throw new Exception();
            }

            _defaultTypeSetProvider = defaultTypeSetProvider;
            _defaultTypeSetVersion = defaultTypeSetVersion;
        }

        public static int Version
        {
            get
            {
                _used = true;
                return _defaultTypeSetVersion;
            }
        }

        public static Func<int, IReadOnlyList<Type>>? Provider
        {
            get
            {
                _used = true;
                return _defaultTypeSetProvider;
            }
        }

        public static IReadOnlyList<Type>? DefaultTypes
        {
            get
            {
                return Provider?.Invoke(Version);
            }
        }
    }
}