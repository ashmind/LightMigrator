using JetBrains.Annotations;

namespace LightMigrator.Framework {
    public interface IMigrationConvention {
        [CanBeNull] string GetVersion([NotNull] IMigration migration);
        [CanBeNull] MigrationEvent? GetEvent([NotNull] IMigration migration);
    }
}