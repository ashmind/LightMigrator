using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using LightMigrator.Framework.FluentInterface;
using LightMigrator.Framework.Internal;

namespace LightMigrator.Engine {
    public interface IDatabase : IDatabaseSyntax, IDefaultHistoryTableDefinitionProvider {
        [NotNull] string ConnectionString { get; }
        [NotNull] IRollbackPlan RollbackPlan { get; }
        [NotNull] IDbConnection OpenConnection();
    }
}