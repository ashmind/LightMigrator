using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Framework {
    [PublicAPI]
    public interface IMigrationContext {
        [NotNull] string PrimaryDatabaseName { get; }
        [NotNull] IReadOnlyDictionary<string, IDatabaseSyntax> Databases { get; }
    }
}
