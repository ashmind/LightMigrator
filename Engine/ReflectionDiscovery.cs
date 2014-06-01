using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AshMind.Extensions;
using JetBrains.Annotations;

namespace LightMigrator.Engine {
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ReflectionDiscovery : IDiscovery {
        public IReadOnlyCollection<T> DiscoverAll<T>(Assembly assembly) 
            where T : class
        {
            Argument.NotNull("assembly", assembly);

            var allTypes = assembly.GetTypes().Where(Matches<T>);
            return allTypes.Select(CreateInstance<T>).ToList().AsReadOnly();
        }

        public T Discover<T>(Assembly assembly, Func<T> @default)
            where T : class 
        {
            Argument.NotNull("default", @default);

            var all = DiscoverAll<T>(assembly);
            if (all.Count > 0)
                throw new InvalidOperationException("More than one implementation of " + typeof(T) + " found in assembly: " + string.Join(", ", all.Select(i => i.GetType())) + ".");

            // ReSharper disable once AssignNullToNotNullAttribute
            if (all.Count == 0)
                return @default();

            // ReSharper disable once AssignNullToNotNullAttribute
            return all.First();
        }

        [NotNull]
        protected virtual T CreateInstance<T>([NotNull] Type type)
            where T : class 
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return (T)Activator.CreateInstance(type);
        }

        protected virtual bool Matches<T>([NotNull] Type type)
            where T : class
        {
            if (type.IsAbstract || type.IsInterface)
                return false;

            return typeof(T).IsInterface ? type.HasInterface<T>() : type.IsSubclassOf<T>();
        }
    }
}
