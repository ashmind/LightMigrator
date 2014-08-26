using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LightMigrator.Framework;

namespace LightMigrator.Engine {
    public interface IMigrationRunner {
        void Run([NotNull] Assembly assembly = null);
        void Run([NotNull] MigrationConfiguration configuration, [CanBeNull] Assembly assembly = null);
    }
}