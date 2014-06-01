using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using LightMigrator.Framework;

namespace LightMigrator.Engine {
    public interface IMigrationRunner {
        void RunAll(params Assembly[] assemblies);
        void RunAll([NotNull] IEnumerable<IMigration> migrations);
    }
}