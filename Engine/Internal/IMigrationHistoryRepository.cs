using System.Collections.Generic;
using JetBrains.Annotations;

namespace LightMigrator.Engine.Internal {
    public interface IMigrationHistoryRepository {
        void Prepare();

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        [NotNull] IReadOnlyCollection<string> GetVersions();
        void Save([NotNull] MigrationInfo info);
    }
}