using System.Collections.Generic;
using JetBrains.Annotations;
using LightMigrator.Framework;

namespace LightMigrator.Engine.Internal {
    public interface IMigrationHistoryRepository {
        [NotNull] MigrationHistoryTableDefinition DefaultTableDefinition { get; }

        void EnsureTable([NotNull] MigrationHistoryTableDefinition table);

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        [NotNull] IReadOnlyCollection<string> GetVersions([NotNull] MigrationHistoryTableDefinition table);
        void Save([NotNull] MigrationHistoryTableDefinition table, [NotNull] MigrationInfo info);
    }
}