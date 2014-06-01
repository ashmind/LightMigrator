using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace LightMigrator.Framework.Internal {
    [ThreadSafe]
    public static class EnsureExtensions {
        [NotNull]
        public static T NotNull<T>(this T value)
            where T : class
        {
            if (value == null)
                throw new Exception("Expected value not to be null. Type: " + typeof(T) + ".");

            return value;
        }
    }
}
