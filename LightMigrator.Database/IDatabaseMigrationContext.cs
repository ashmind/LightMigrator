using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Database.Internal;

namespace LightMigrator.Database {
    public interface IDatabaseMigrationContext : IMigrationContext {
        [NotNull] string PrimaryDatabaseName { get; }
        [NotNull] IReadOnlyDictionary<string, IDatabase> Databases { get; }
    }
}
