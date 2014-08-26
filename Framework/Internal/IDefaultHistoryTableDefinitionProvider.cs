using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;

namespace LightMigrator.Framework.Internal {
    public interface IDefaultHistoryTableDefinitionProvider : IDatabaseSyntax {
        [NotNull]
        MigrationHistoryTableDefinition GetDefaultHistoryTableDefinition();
    }
}
