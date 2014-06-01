using JetBrains.Annotations;

namespace LightMigrator.Framework.Conventions {
    [ThreadSafe]
    public interface IMigrationConvention {
        [NotNull] MigrationInfo GetInfo([NotNull] IMigration migration);
    }
}