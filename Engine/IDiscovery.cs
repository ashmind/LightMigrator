using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace LightMigrator.Engine {
    public interface IDiscovery {
        [NotNull]
        IReadOnlyCollection<T> DiscoverAll<T>([NotNull] Assembly assembly) 
            where T : class;

        [NotNull]
        T Discover<T>([NotNull] Assembly assembly, [NotNull] Func<T> @default)
            where T : class;
    }
}