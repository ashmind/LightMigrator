using JetBrains.Annotations;

namespace LightMigrator {
    public interface IMigrationConvention {
        [CanBeNull] string GetVersion([NotNull] IMigration migration);
        [CanBeNull] MigrationEvent? GetEvent([NotNull] IMigration migration);
    }
}