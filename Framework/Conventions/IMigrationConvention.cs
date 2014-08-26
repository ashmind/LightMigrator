using JetBrains.Annotations;

namespace LightMigrator.Framework.Conventions {
    public interface IMigrationConvention {
        [NotNull] MigrationInfo GetInfo([NotNull] IMigration migration);
    }
}